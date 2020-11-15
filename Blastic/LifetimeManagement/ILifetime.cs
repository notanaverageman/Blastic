using System.Threading;
using System.Threading.Tasks;
using Blastic.Commanding;
using Blastic.LifetimeManagement.Contexts;
using Blastic.Reactive;

namespace Blastic.LifetimeManagement
{
	public interface ILifetime
	{
		IReadOnlyReactiveProperty<bool> IsInitialized { get; }
		IReadOnlyReactiveProperty<bool> IsActive { get; }

		IReadOnlyReactiveProperty<bool> IsActivating { get; }

		Command<InitializationContext> Initialization { get; }
		
		Command<ClosureContext> Closure { get; }
		Command<ClosureContext> CanClose { get; }

		Command<ActivationContext> Activation { get; }
		Command<DeactivationContext> Deactivation { get; }

		Task Initialize(
			CancellationToken cancellationToken = default,
			InitializationContext? context = default);

		Task Activate(
			CancellationToken cancellationToken = default,
			ActivationContext? context = default);

		Task Deactivate(
			CancellationToken cancellationToken = default,
			DeactivationContext? context = default);

		Task Close(
			CancellationToken cancellationToken = default,
			ClosureContext? context = default);
	}
}