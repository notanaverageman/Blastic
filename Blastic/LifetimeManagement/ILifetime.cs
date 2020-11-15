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

		Command<InitializationContext> Initialize { get; }
		
		Command<ClosureContext> Close { get; }
		Command<ClosureContext> CanClose { get; }

		Command<ActivationContext> Activate { get; }
		Command<DeactivationContext> Deactivate { get; }
	}
}