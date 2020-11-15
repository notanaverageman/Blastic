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

		public Command<InitializationContext> Initialization { get; }

		public Command<ClosureContext> Closure { get; }
		public Command<ClosureContext> CanClose { get; }

		public Command<ActivationContext> Activation { get; }
		public Command<DeactivationContext> Deactivation { get; }

		public Lifetime()
		{
			_isInitialized = new ReactiveProperty<bool>();
			_isActive = new ReactiveProperty<bool>();
			_isActivating = new ReactiveProperty<bool>();

			Initialization = IsInitialized
				.Select(x => !x)
				.ToCommand<InitializationContext>()
				.WithSubscribe(AfterInitialization, Order.AbsoluteMaximum);

			CanClose = new Command<ClosureContext>();

			Closure = IsInitialized
				.ToCommand<ClosureContext>()
				.WithSubscribe(BeforeClosure, Order.AbsoluteMinimum)
				.WithSubscribe(AfterClosure, Order.AbsoluteMaximum);

			Activation = IsActive
				.Select(x => !x)
				.ToCommand<ActivationContext>()
				.WithSubscribe(BeforeActivation, Order.AbsoluteMinimum)
				.WithSubscribe(AfterActivation, Order.AbsoluteMaximum);

			Deactivation = IsActive
				.ToCommand<DeactivationContext>()
				.WithSubscribe(AfterDeactivation, Order.AbsoluteMaximum);
		}

		public async Task Initialize(
			CancellationToken cancellationToken,
			InitializationContext? context)
		{
			context ??= new InitializationContext();
			await Initialization.Execute(context, cancellationToken);
		}

		public async Task Activate(
			CancellationToken cancellationToken,
			ActivationContext? context)
		{
			context ??= new ActivationContext();
			await Activation.Execute(context, cancellationToken);
		}

		public async Task Deactivate(
			CancellationToken cancellationToken,
			DeactivationContext? context)
		{
			context ??= new DeactivationContext();
			await Deactivation.Execute(context, cancellationToken);
		}

		public async Task Close(
			CancellationToken cancellationToken,
			ClosureContext? context)
		{
			context ??= new ClosureContext();
			await Closure.Execute(context, cancellationToken);
		}

		private Task AfterInitialization()
		{
			_isInitialized.Value = true;

			return Task.CompletedTask;
		}

		private async Task BeforeClosure(ClosureContext context, CancellationToken cancellationToken)
		{
			await CanClose.Execute(context, cancellationToken);

			if (context.IsCancelled)
			{
				return;
			}

			DeactivationContext deactivationContext = new DeactivationContext();

			await Deactivation.Execute(deactivationContext, cancellationToken);
		}

		private Task AfterClosure()
		{
			_isInitialized.Value = false;

			return Task.CompletedTask;
		}

		private async Task BeforeActivation(CancellationToken cancellationToken)
		{
			_isActivating.Value = true;

			InitializationContext initializationContext = new InitializationContext();

			await Initialization.Execute(initializationContext, cancellationToken);
		}

		private Task AfterActivation()
		{
			_isActivating.Value = false;
			_isActive.Value = true;

			return Task.CompletedTask;
		}

		private Task AfterDeactivation()
		{
			_isActive.Value = false;

			return Task.CompletedTask;
		}
	}
}