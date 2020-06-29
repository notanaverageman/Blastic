using System.Threading;
using System.Threading.Tasks;
using Blastic.Commanding;
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

			CommandContext<ActivationContext> context = new CommandContext<ActivationContext>(
				new ActivationContext(),
				cancellationToken);

			await item.Lifetime.Activate.Execute(context);
		}

		public async Task Deactivate(T item, CancellationToken cancellationToken = default)
		{
			if (!Items.Contains(item))
			{
				Items.Add(item);
			}

			CommandContext<DeactivationContext> context = new CommandContext<DeactivationContext>(
				new DeactivationContext(),
				cancellationToken);

			await item.Lifetime.Deactivate.Execute(context);
		}
	}
}