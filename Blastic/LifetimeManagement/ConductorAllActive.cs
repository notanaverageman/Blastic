using System.Threading;
using System.Threading.Tasks;

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

			await item.Lifetime.Activate(cancellationToken);
		}

		public async Task Deactivate(T item, CancellationToken cancellationToken = default)
		{
			if (!Items.Contains(item))
			{
				Items.Add(item);
			}

			await item.Lifetime.Deactivate(cancellationToken);
		}
	}
}