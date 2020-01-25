using System.Reactive.Linq;
using System.Threading.Tasks;
using Blastic.Commanding;
using Blastic.LifetimeManagement.Contexts;
using Blastic.Ordering;
using Blastic.Reactive;

namespace Blastic.LifetimeManagement
{
	public class Lifetime : ILifetime
	{
		private readonly IReactiveProperty<bool> _isInitialized;
		private readonly IReactiveProperty<bool> _isActive;
		private readonly IReactiveProperty<bool> _isActivating;

		public IReadOnlyReactiveProperty<bool> IsInitialized => _isInitialized;
		public IReadOnlyReactiveProperty<bool> IsActive => _isActive;
		public IReadOnlyReactiveProperty<bool> IsActivating => _isActivating;

		public AsyncCommand<InitializationContext> Initialize { get; }

		public AsyncCommand<ClosureContext> Close { get; }
		public AsyncCommand<ClosureContext> CanClose { get; }

		public AsyncCommand<ActivationContext> Activate { get; }
		public AsyncCommand<DeactivationContext> Deactivate { get; }

		public Lifetime()
		{
			_isInitialized = new ReactiveProperty<bool>();
			_isActive = new ReactiveProperty<bool>();
			_isActivating = new ReactiveProperty<bool>();

			Initialize = IsInitialized
				.Select(x => !x)
				.ToAsyncCommand<InitializationContext>()
				.WithSubscribe(PostInitialize, Order.AbsoluteMaximum);

			CanClose = new AsyncCommand<ClosureContext>();

			Close = IsInitialized
				.ToAsyncCommand<ClosureContext>()
				.WithSubscribe(PreClose, Order.AbsoluteMinimum)
				.WithSubscribe(PostClose, Order.AbsoluteMaximum);

			Activate = IsActive
				.Select(x => !x)
				.ToAsyncCommand<ActivationContext>()
				.WithSubscribe(PreActivate, Order.AbsoluteMinimum)
				.WithSubscribe(PostActivate, Order.AbsoluteMaximum);

			Deactivate = IsActive
				.ToAsyncCommand<DeactivationContext>()
				.WithSubscribe(PostDeactivate, Order.AbsoluteMaximum);
		}

		private Task PostInitialize(AsyncCommandContext<InitializationContext> context)
		{
			_isInitialized.Value = true;
			return Task.CompletedTask;
		}

		private async Task PreClose(AsyncCommandContext<ClosureContext> context)
		{
			ClosureContext closureContext = context.Parameter;
			await CanClose.Execute(closureContext);

			context.ContinueExecution = closureContext.CanClose;

			if (!context.ContinueExecution)
			{
				return;
			}
			
			DeactivationContext deactivationContext = new DeactivationContext(closureContext.CancellationToken);
			await Deactivate.Execute(deactivationContext);
		}

		private Task PostClose(AsyncCommandContext<ClosureContext> context)
		{
			_isInitialized.Value = false;
			return Task.CompletedTask;
		}

		private async Task PreActivate(AsyncCommandContext<ActivationContext> context)
		{
			_isActivating.Value = true;

			InitializationContext initializationContext = new InitializationContext(context.Parameter.CancellationToken);
			await Initialize.Execute(initializationContext);
		}

		private Task PostActivate(AsyncCommandContext<ActivationContext> context)
		{
			_isActivating.Value = false;
			_isActive.Value = true;
			return Task.CompletedTask;
		}

		private Task PostDeactivate(AsyncCommandContext<DeactivationContext> context)
		{
			_isActive.Value = false;
			return Task.CompletedTask;
		}
	}
}