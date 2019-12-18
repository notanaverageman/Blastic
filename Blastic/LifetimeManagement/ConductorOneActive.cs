using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Execution;
using Blastic.LifetimeManagement.Contexts;
using Blastic.Reactive;
using ActivationContext = Blastic.LifetimeManagement.Contexts.ActivationContext;

namespace Blastic.LifetimeManagement
{
	public class ConductorOneActive<T> : ConductorBase<T> where T : IHasLifetime
	{
		private readonly IReadOnlyReactiveProperty<T> _previousActiveItem;

		public ReactiveProperty<T> ActiveItem { get; }

		public ConductorOneActive(ExecutionContextFactory executionContextFactory) : base(executionContextFactory)
		{
			LifetimeChainOptions.ActivateChildrenOnSelfActivation = false;

			ActiveItem = new ReactiveProperty<T>();

			_previousActiveItem = ActiveItem
				.Scan<T, (T Previous, T Current)>(
					(default, default),
					(accumulator, current) => (accumulator.Current, current))
				.Select(x => x.Previous)
				.ToReadOnlyReactiveProperty();

			ActiveItem.Subscribe(async x =>
			{
				await Activate(ActiveItem.Value);
			});

			InitializeChildLifetimeSubscriptions();
		}

		public async Task Activate(T item, CancellationToken cancellationToken = default)
		{
			if (item != null && !Items.Contains(item))
			{
				Items.Add(item);
			}

			bool isActivating = Lifetime.IsActivating.Value;
			bool isActive = Lifetime.IsActive.Value;

			if (!(isActivating || isActive))
			{
				return;
			}

			// This does not cause a stack overflow since equality comparer in ActiveItem.Value
			// returns early if we set the same item.
			ActiveItem.Value = item;
			await ChangeActiveItem(cancellationToken);
		}

		public async Task Close(T item, CancellationToken cancellationToken = default)
		{
			ClosureContext context = new ClosureContext(cancellationToken);
			await item.Lifetime.Close.Execute(context);

			if (!context.CanClose)
			{
				return;
			}
			
			if (Equals(item, ActiveItem.Value))
			{
				ActiveItem.Value = _previousActiveItem.Value;
			}

			Items.Remove(item);
		}

		private async Task ChangeActiveItem(CancellationToken cancellationToken)
		{
			IHasLifetime previousActiveItem = _previousActiveItem.Value;
			IHasLifetime activeItem = ActiveItem.Value;

			if (Equals(previousActiveItem, activeItem))
			{
				return;
			}

			if (previousActiveItem != null)
			{
				DeactivationContext context = new DeactivationContext(cancellationToken);
				await previousActiveItem.Lifetime.Deactivate.Execute(context);
			}

			if (activeItem != null)
			{
				ActivationContext activationContext = new ActivationContext(cancellationToken);
				await activeItem.Lifetime.Activate.Execute(activationContext);
			}
		}
	}
}