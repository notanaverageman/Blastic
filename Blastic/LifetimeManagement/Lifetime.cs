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
		private readonly IReactiveProperty<bool> _isInitializing;
		private readonly IReactiveProperty<bool> _isActivating;
		private readonly IReactiveProperty<bool> _isDeactivating;
		private readonly IReactiveProperty<bool> _isClosing;

		/// <inheritdoc />
		public IReadOnlyReactiveProperty<bool> IsInitialized => _isInitialized;

		/// <inheritdoc />
		public IReadOnlyReactiveProperty<bool> IsActive => _isActive;

		/// <inheritdoc />
		public IReadOnlyReactiveProperty<bool> IsInitializing => _isInitializing;

		/// <inheritdoc />
		public IReadOnlyReactiveProperty<bool> IsActivating => _isActivating;

		/// <inheritdoc />
		public IReadOnlyReactiveProperty<bool> IsDeactivating => _isDeactivating;

		/// <inheritdoc />
		public IReadOnlyReactiveProperty<bool> IsClosing => _isClosing;

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
			_isInitializing = new ReactiveProperty<bool>(false);
			_isActivating = new ReactiveProperty<bool>(false);
			_isDeactivating = new ReactiveProperty<bool>(false);
			_isClosing = new ReactiveProperty<bool>(false);

			Initialization = IsInitialized
				.And(_isInitializing.Negate())
				.Select(x => !x)
				.ToCommand<InitializationContext>()
				.WithSubscribe(BeforeInitialization, Order.AbsoluteMinimum)
				.WithSubscribe(AfterInitialization, Order.AbsoluteMaximum);

			CanClose = new Command<ClosureContext>();

			Closure = IsInitialized
				.And(_isClosing.Negate())
				.ToCommand<ClosureContext>()
				.WithSubscribe(BeforeClosure, Order.AbsoluteMinimum)
				.WithSubscribe(AfterClosure, Order.AbsoluteMaximum);

			Activation = IsActive
				.And(_isActivating)
				.Negate()
				.ToCommand<ActivationContext>()
				.WithSubscribe(BeforeActivation, Order.AbsoluteMinimum)
				.WithSubscribe(AfterActivation, Order.AbsoluteMaximum);

			Deactivation = IsActive
				.And(_isDeactivating.Negate())
				.ToCommand<DeactivationContext>()
				.WithSubscribe(BeforeDeactivation, Order.AbsoluteMinimum)
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

		private void BeforeInitialization()
		{
			_isInitializing.Value = true;
		}

		private void AfterInitialization()
		{
			_isInitializing.Value = false;
			_isInitialized.Value = true;
		}

		private void BeforeClosure(ClosureContext? context, CancellationToken cancellationToken)
		{
			CanClose.Execute(context, cancellationToken);

			if (context?.IsCancelled == true)
			{
				return;
			}

			_isClosing.Value = true;

			DeactivationContext deactivationContext = new();
			Deactivation.Execute(deactivationContext, cancellationToken);
		}

		private void AfterClosure()
		{
			_isClosing.Value = false;
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

		private void BeforeDeactivation()
		{
			_isDeactivating.Value = true;
		}

		private void AfterDeactivation()
		{
			_isDeactivating.Value = false;
			_isActive.Value = false;
		}
	}
}