using System.Threading;
using System.Threading.Tasks;
using Blastic.Execution;
using Blastic.LifetimeManagement.Contexts;

namespace Blastic.LifetimeManagement
{
	public class ConductorAllActive : ConductorBase
	{
		public ConductorAllActive(ExecutionContextFactory executionContextFactory) : base(executionContextFactory)
		{
			InitializeChildLifetimeSubscriptions();
		}

		public async Task Activate(IHasLifetime item, CancellationToken cancellationToken = default)
		{
			if (!Lifetime.IsActive.Value)
			{
				return;
			}

			if (!Items.Contains(item))
			{
				Items.Add(item);
			}

			ActivationContext context = new ActivationContext(cancellationToken);
			await item.Lifetime.Activate.Execute(context);
		}

		public async Task Deactivate(IHasLifetime item, CancellationToken cancellationToken = default)
		{
			if (!Items.Contains(item))
			{
				Items.Add(item);
			}

			DeactivationContext context = new DeactivationContext(cancellationToken);
			await item.Lifetime.Deactivate.Execute(context);
		}
	}
}