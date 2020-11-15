using System.Reactive.Linq;
using System.Threading;
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

		public Command<InitializationContext> Initialize { get; }

		public Command<ClosureContext> Close { get; }
		public Command<ClosureContext> CanClose { get; }

		public Command<ActivationContext> Activate { get; }
		public Command<DeactivationContext> Deactivate { get; }

		public Lifetime()
		{
			_isInitialized = new ReactiveProperty<bool>();
			_isActive = new ReactiveProperty<bool>();
			_isActivating = new ReactiveProperty<bool>();

			Initialize = IsInitialized
				.Select(x => !x)
				.ToCommand<InitializationContext>()
				.WithSubscribe(PostInitialize, Order.AbsoluteMaximum);

			CanClose = new Command<ClosureContext>();

			Close = IsInitialized
				.ToCommand<ClosureContext>()
				.WithSubscribe(PreClose, Order.AbsoluteMinimum)
				.WithSubscribe(PostClose, Order.AbsoluteMaximum);

			Activate = IsActive
				.Select(x => !x)
				.ToCommand<ActivationContext>()
				.WithSubscribe(PreActivate, Order.AbsoluteMinimum)
				.WithSubscribe(PostActivate, Order.AbsoluteMaximum);

			Deactivate = IsActive
				.ToCommand<DeactivationContext>()
				.WithSubscribe(PostDeactivate, Order.AbsoluteMaximum);
		}

		private Task PostInitialize()
		{
			_isInitialized.Value = true;

			return Task.CompletedTask;
		}

		private async Task PreClose(ClosureContext context, CancellationToken cancellationToken)
		{
			await CanClose.Execute(context, cancellationToken);

			if (context.IsCancelled)
			{
				return;
			}

			DeactivationContext deactivationContext = new DeactivationContext();

			await Deactivate.Execute(deactivationContext, cancellationToken);
		}

		private Task PostClose()
		{
			_isInitialized.Value = false;

			return Task.CompletedTask;
		}

		private async Task PreActivate(CancellationToken cancellationToken)
		{
			_isActivating.Value = true;

			InitializationContext initializationContext = new InitializationContext();

			await Initialize.Execute(initializationContext, cancellationToken);
		}

		private Task PostActivate()
		{
			_isActivating.Value = false;
			_isActive.Value = true;

			return Task.CompletedTask;
		}

		private Task PostDeactivate()
		{
			_isActive.Value = false;

			return Task.CompletedTask;
		}
	}
}