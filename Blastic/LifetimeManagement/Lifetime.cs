using System.Reactive.Linq;
using System.Threading;
using Blastic.Commanding;
using Blastic.LifetimeManagement.Contexts;
using Blastic.Ordering;
using Blastic.Reactive;

namespace Blastic.LifetimeManagement
{
	/// <summary>
	/// Default implementation of <see cref="ILifetime"/>.
	/// </summary>
	public class Lifetime : ILifetime
	{
		private readonly IReactiveProperty<bool> _isInitialized;
		private readonly IReactiveProperty<bool> _isActive;
		private readonly IReactiveProperty<bool> _isActivating;

		/// <inheritdoc />
		public IReadOnlyReactiveProperty<bool> IsInitialized => _isInitialized;

		/// <inheritdoc />
		public IReadOnlyReactiveProperty<bool> IsActive => _isActive;

		/// <inheritdoc />
		public IReadOnlyReactiveProperty<bool> IsActivating => _isActivating;

		/// <inheritdoc />
		public Command<InitializationContext> Initialization { get; }

		/// <inheritdoc />
		public Command<ClosureContext> Closure { get; }

		/// <inheritdoc />
		public Command<ClosureContext> CanClose { get; }

		/// <inheritdoc />
		public Command<ActivationContext> Activation { get; }

		/// <inheritdoc />
		public Command<DeactivationContext> Deactivation { get; }

		/// <summary>
		/// Creates a new <see cref="Lifetime"/> object.
		/// </summary>
		public Lifetime()
		{
			_isInitialized = new ReactiveProperty<bool>(false);
			_isActive = new ReactiveProperty<bool>(false);
			_isActivating = new ReactiveProperty<bool>(false);

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

		/// <inheritdoc />
		public void Initialize(
			CancellationToken cancellationToken,
			InitializationContext? context)
		{
			context ??= new InitializationContext();
			Initialization.Execute(context, cancellationToken);
		}

		/// <inheritdoc />
		public void Activate(
			CancellationToken cancellationToken,
			ActivationContext? context)
		{
			context ??= new ActivationContext();
			Activation.Execute(context, cancellationToken);
		}

		/// <inheritdoc />
		public void Deactivate(
			CancellationToken cancellationToken,
			DeactivationContext? context)
		{
			context ??= new DeactivationContext();
			Deactivation.Execute(context, cancellationToken);
		}

		/// <inheritdoc />
		public void Close(
			CancellationToken cancellationToken,
			ClosureContext? context)
		{
			context ??= new ClosureContext();
			Closure.Execute(context, cancellationToken);
		}

		private void AfterInitialization()
		{
			_isInitialized.Value = true;
		}

		private void BeforeClosure(ClosureContext? context, CancellationToken cancellationToken)
		{
			CanClose.Execute(context, cancellationToken);

			if (context?.IsCancelled == true)
			{
				return;
			}

			DeactivationContext deactivationContext = new();

			Deactivation.Execute(deactivationContext, cancellationToken);
		}

		private void AfterClosure()
		{
			_isInitialized.Value = false;
		}

		private void BeforeActivation(CancellationToken cancellationToken)
		{
			_isActivating.Value = true;

			InitializationContext initializationContext = new();

			Initialization.Execute(initializationContext, cancellationToken);
		}

		private void AfterActivation()
		{
			_isActivating.Value = false;
			_isActive.Value = true;
		}

		private void AfterDeactivation()
		{
			_isActive.Value = false;
		}
	}
}