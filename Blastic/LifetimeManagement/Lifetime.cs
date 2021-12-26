using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
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
		public AsyncCommand<InitializationContext> Initialization { get; }

		/// <inheritdoc />
		public AsyncCommand<ClosureContext> Closure { get; }

		/// <inheritdoc />
		public AsyncCommand<ClosureContext> CanClose { get; }

		/// <inheritdoc />
		public AsyncCommand<ActivationContext> Activation { get; }

		/// <inheritdoc />
		public AsyncCommand<DeactivationContext> Deactivation { get; }

		/// <summary>
		/// Creates a new <see cref="Lifetime"/> object.
		/// </summary>
		public Lifetime()
		{
			_isInitialized = new ReactiveProperty<bool>();
			_isActive = new ReactiveProperty<bool>();
			_isActivating = new ReactiveProperty<bool>();

			Initialization = IsInitialized
				.Select(x => !x)
				.ToAsyncCommand<InitializationContext>()
				.WithSubscribe(AfterInitialization, Order.AbsoluteMaximum);

			CanClose = new AsyncCommand<ClosureContext>();

			Closure = IsInitialized
				.ToAsyncCommand<ClosureContext>()
				.WithSubscribe(BeforeClosure, Order.AbsoluteMinimum)
				.WithSubscribe(AfterClosure, Order.AbsoluteMaximum);

			Activation = IsActive
				.Select(x => !x)
				.ToAsyncCommand<ActivationContext>()
				.WithSubscribe(BeforeActivation, Order.AbsoluteMinimum)
				.WithSubscribe(AfterActivation, Order.AbsoluteMaximum);

			Deactivation = IsActive
				.ToAsyncCommand<DeactivationContext>()
				.WithSubscribe(AfterDeactivation, Order.AbsoluteMaximum);
		}

		/// <inheritdoc />
		public async Task Initialize(
			CancellationToken cancellationToken,
			InitializationContext? context)
		{
			context ??= new InitializationContext();
			await Initialization.Execute(context, cancellationToken);
		}

		/// <inheritdoc />
		public async Task Activate(
			CancellationToken cancellationToken,
			ActivationContext? context)
		{
			context ??= new ActivationContext();
			await Activation.Execute(context, cancellationToken);
		}

		/// <inheritdoc />
		public async Task Deactivate(
			CancellationToken cancellationToken,
			DeactivationContext? context)
		{
			context ??= new DeactivationContext();
			await Deactivation.Execute(context, cancellationToken);
		}

		/// <inheritdoc />
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

		private async Task BeforeClosure(ClosureContext? context, CancellationToken cancellationToken)
		{
			await CanClose.Execute(context, cancellationToken);

			if (context?.IsCancelled == true)
			{
				return;
			}

			DeactivationContext deactivationContext = new();

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

			InitializationContext initializationContext = new();

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