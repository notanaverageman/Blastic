using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Execution;
using Blastic.LifetimeManagement.Contexts;
using Reactive.Bindings;
using ActivationContext = Blastic.LifetimeManagement.Contexts.ActivationContext;

namespace Blastic.LifetimeManagement
{
	public class ConductorOneActive : ConductorBase
	{
		private readonly IReadOnlyReactiveProperty<IHasLifetime> _previousActiveItem;

		public IReactiveProperty<IHasLifetime> ActiveItem { get; }

		public ConductorOneActive(ExecutionContextFactory executionContextFactory) : base(executionContextFactory)
		{
			LifetimeChainOptions.ActivateChildrenOnSelfActivation = false;

			ActiveItem = new ReactivePropertySlim<IHasLifetime>();
			
			_previousActiveItem = ActiveItem
				.Scan<IHasLifetime, IHasLifetime>(default, (accumulator, current) => current)
				.ToReadOnlyReactiveProperty();

			ActiveItem.Subscribe(async x =>
			{
				await ChangeActiveItem(x, default);
			});

			InitializeChildLifetimeSubscriptions();
		}

		public async Task Activate(IHasLifetime item, CancellationToken cancellationToken = default)
		{
			if (!Items.Contains(item))
			{
				Items.Add(item);
			}

			if (!Lifetime.IsActive.Value)
			{
				return;
			}

			await ChangeActiveItem(item, cancellationToken);
		}

		public async Task Close(IHasLifetime item, CancellationToken cancellationToken = default)
		{
			if (Equals(item, ActiveItem.Value))
			{
				await ChangeActiveItem(_previousActiveItem.Value, cancellationToken);
			}

			ClosureContext context = new ClosureContext(cancellationToken);
			await item.Lifetime.Close.Execute(context);

			if (!context.CanClose)
			{
				return;
			}

			Items.Remove(item);
		}

		private async Task ChangeActiveItem(
			IHasLifetime newActiveItem,
			CancellationToken cancellationToken)
		{
			if (Equals(newActiveItem, ActiveItem.Value))
			{
				return;
			}

			if (ActiveItem.Value != null)
			{
				DeactivationContext context = new DeactivationContext(cancellationToken);
				await ActiveItem.Value.Lifetime.Deactivate.Execute(context);
			}

			if (newActiveItem != null)
			{
				ActiveItem.Value = newActiveItem;

				InitializationContext initializationContext = new InitializationContext(cancellationToken);
				ActivationContext activationContext = new ActivationContext(cancellationToken);
				
				await newActiveItem.Lifetime.Initialize.Execute(initializationContext);
				await newActiveItem.Lifetime.Activate.Execute(activationContext);
			}
		}
	}
}