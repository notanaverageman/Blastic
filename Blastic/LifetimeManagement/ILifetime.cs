using Blastic.LifetimeManagement.Contexts;
using Blastic.Reactive;
using Reactive.Bindings;

namespace Blastic.LifetimeManagement
{
	public interface ILifetime
	{
		IReadOnlyReactiveProperty<bool> IsInitialized { get; }
		IReadOnlyReactiveProperty<bool> IsActive { get; }

		AsyncCommand<InitializationContext> Initialize { get; }
		
		AsyncCommand<ClosureContext> Close { get; }
		AsyncCommand<ClosureContext> CanClose { get; }

		AsyncCommand<ActivationContext> Activate { get; }
		AsyncCommand<DeactivationContext> Deactivate { get; }
	}
}