using System.Threading;
using System.Threading.Tasks;
using Blastic.LifetimeManagement.Contexts;

namespace Blastic.LifetimeManagement
{
	public class ConductorAllActive<T> : ConductorBase<T> where T : IHasLifetime
	{
		public ConductorAllActive()
		{
			InitializeChildLifetimeSubscriptions();
		}

		public async Task Activate(T item, CancellationToken cancellationToken = default)
		{
			if (!Lifetime.IsActive.Value)
			{
				return;
			}

			if (!Items.Contains(item))
			{
				Items.Add(item);
			}

			ActivationContext context = new ActivationContext();

			await item.Lifetime.Activate.Execute(context, cancellationToken);
		}

		public async Task Deactivate(T item, CancellationToken cancellationToken = default)
		{
			if (!Items.Contains(item))
			{
				Items.Add(item);
			}

			DeactivationContext context = new DeactivationContext();

			await item.Lifetime.Deactivate.Execute(context, cancellationToken);
		}
	}
}