using Blastic.LifetimeManagement.Contexts;
using Blastic.Reactive;

namespace Blastic.LifetimeManagement
{
	public interface ILifetime
	{
		IReadOnlyReactiveProperty<bool> IsInitialized { get; }
		IReadOnlyReactiveProperty<bool> IsActive { get; }

		IReadOnlyReactiveProperty<bool> IsActivating { get; }

		AsyncCommand<InitializationContext> Initialize { get; }
		
		AsyncCommand<ClosureContext> Close { get; }
		AsyncCommand<ClosureContext> CanClose { get; }

		AsyncCommand<ActivationContext> Activate { get; }
		AsyncCommand<DeactivationContext> Deactivate { get; }
	}
}