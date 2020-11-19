using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blastic.LifetimeManagement.Contexts;
using Blastic.Reactive;

namespace Blastic.LifetimeManagement
{
	public class ConductorOneActive<T> : ConductorBase<T> where T : class, IHasLifetime
	{
		private readonly IReadOnlyReactiveProperty<T?> _previousActiveItem;

		public ReactiveProperty<T?> ActiveItem { get; }
		public ReactiveProperty<int> ActiveItemIndex { get; }

		public ConductorOneActive()
		{
			LifetimeChainOptions.ActivateChildrenOnSelfActivation = false;

			ActiveItem = new ReactiveProperty<T?>();
			ActiveItemIndex = new ReactiveProperty<int>(-1);

			_previousActiveItem = ActiveItem
				.Scan<T?, (T? Previous, T? Current)>(
					(default, default),
					(accumulator, current) => (accumulator.Current, current))
				.Select(x => x.Previous)
				.ToReadOnlyReactiveProperty();

			ActiveItem.Subscribe(async x =>
			{
				await Activate(x);
			});

			ActiveItemIndex.Subscribe(async x =>
			{
				if (x < 0 || x >= Items.Count)
				{
					await Activate(null);
				}
				else
				{
					await Activate(Items[x]);
				}
			});

			InitializeChildLifetimeSubscriptions();
		}

		public async Task Activate(T? item, CancellationToken cancellationToken = default)
		{
			int index = item != null ? Items.IndexOf(item) : -1;

			if (item != null && index < 0)
			{
				Items.Add(item);
				index = Items.Count - 1;
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
			ActiveItemIndex.Value = index;

			await ChangeActiveItem(cancellationToken);
		}

		public async Task Close(
			T item,
			CancellationToken cancellationToken = default,
			bool dialogResult = false)
		{
			ClosureContext context = new ClosureContext(dialogResult);

			await item.Lifetime.Close(cancellationToken, context);

			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}

			if (context.IsCancelled)
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
			IHasLifetime? previousActiveItem = _previousActiveItem.Value;
			IHasLifetime? activeItem = ActiveItem.Value;

			if (Equals(previousActiveItem, activeItem))
			{
				return;
			}

			if (previousActiveItem != null)
			{
				await previousActiveItem.Lifetime.Deactivate(cancellationToken);
			}

			if (activeItem != null)
			{
				await activeItem.Lifetime.Activate(cancellationToken);
			}
		}
	}
}