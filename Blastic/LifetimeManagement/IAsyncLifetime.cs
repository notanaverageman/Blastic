using System.Threading;
using System.Threading.Tasks;
using Blastic.Commanding;
using Blastic.LifetimeManagement.Contexts;
using Blastic.Ordering;
using Blastic.Reactive;

namespace Blastic.LifetimeManagement
{
	/// <summary>
	/// Contains observable properties to track the lifecycle of an object and commands
	/// to transition between different states.
	/// </summary>
	public interface IAsyncLifetime
	{
		/// <summary>
		/// An observable property that returns true if the object is initialized.
		/// </summary>
		IReadOnlyReactiveProperty<bool> IsInitialized { get; }

		/// <summary>
		/// An observable property that returns true if the object is activated.
		/// </summary>
		IReadOnlyReactiveProperty<bool> IsActive { get; }

		/// <summary>
		/// An observable property that returns true if the object is currently being activated.
		/// </summary>
		IReadOnlyReactiveProperty<bool> IsActivating { get; }

		/// <summary>
		/// Command that is executed to initialize the object. Subscribe to this command
		/// to execute actions when the object is being initialized.
		/// </summary>
		/// <remarks>
		/// Do not use <see cref="Order.AbsoluteMaximum"/> while subscribing as it is
		/// reserved for internal use.
		/// </remarks>
		AsyncCommand<InitializationContext> Initialization { get; }

		/// <summary>
		/// Command that is executed to deinitialize the object. Subscribe to this command
		/// to execute actions when the object is being deinitialized.
		/// </summary>
		/// <remarks>
		/// Do not use <see cref="Order.AbsoluteMaximum"/> or <see cref="Order.AbsoluteMaximum"/>
		/// while subscribing as it is reserved for internal use.
		/// </remarks>
		AsyncCommand<ClosureContext> Closure { get; }

		/// <summary>
		/// Command that is executed to determine if the object can be deinitialized.
		/// Subscribe to this command to be able to cancel the deinitialization process.
		/// </summary>
		AsyncCommand<ClosureContext> CanClose { get; }

		/// <summary>
		/// Command that is executed to activate the object. Subscribe to this command
		/// to execute actions when the object is being activated.
		/// </summary>
		/// <remarks>
		/// Do not use <see cref="Order.AbsoluteMaximum"/> or <see cref="Order.AbsoluteMaximum"/>
		/// while subscribing as it is reserved for internal use.
		/// </remarks>
		AsyncCommand<ActivationContext> Activation { get; }

		/// <summary>
		/// Command that is executed to deactivate the object. Subscribe to this command
		/// to execute actions when the object is being deactivated.
		/// </summary>
		/// <remarks>
		/// Do not use <see cref="Order.AbsoluteMaximum"/> while subscribing as it is
		/// reserved for internal use.
		/// </remarks>
		AsyncCommand<DeactivationContext> Deactivation { get; }

		/// <summary>
		/// A method that executes the <see cref="Initialization"/> command.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="context">The parameter to pass to the command.</param>
		/// <returns>A task to be awaited.</returns>
		Task Initialize(
			CancellationToken cancellationToken = default,
			InitializationContext? context = default);

		/// <summary>
		/// A method that executes the <see cref="Activation"/> command.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="context">The parameter to pass to the command.</param>
		/// <returns>A task to be awaited.</returns>
		Task Activate(
			CancellationToken cancellationToken = default,
			ActivationContext? context = default);

		/// <summary>
		/// A method that executes the <see cref="Deactivation"/> command.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="context">The parameter to pass to the command.</param>
		/// <returns>A task to be awaited.</returns>
		Task Deactivate(
			CancellationToken cancellationToken = default,
			DeactivationContext? context = default);

		/// <summary>
		/// A method that executes the <see cref="Closure"/> command.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="context">The parameter to pass to the command.</param>
		/// <returns>A task to be awaited.</returns>
		Task Close(
			CancellationToken cancellationToken = default,
			ClosureContext? context = default);
	}
}