using System;
using System.Collections.Generic;
using System.Threading;
using Blastic.LifetimeManagement.Contexts;
using Blastic.Reactive;
using DynamicData;

namespace Blastic.LifetimeManagement
{
	/// <summary>
	/// A class that can have many items and at most only one of them can be active at a time.
	/// </summary>
	/// <typeparam name="T">A type with a lifetime.</typeparam>
	public class ConductorOneActive<T> : ConductorBase<T> where T : class, IHasLifetime
	{
		private readonly List<T?> _activeItemStack;

		/// <summary>
		/// An observable property that holds the currently active item. Has a null value
		/// if none of the items are active.
		/// <remarks>
		/// Setting the value of this property will change <see cref="ActiveItemIndex"/>.
		/// </remarks>
		/// </summary>
		public IReactiveProperty<T?> ActiveItem { get; }

		/// <summary>
		/// An observable property that holds the currently active item's index. Has a value
		/// of -1 if none of the items are active.
		/// </summary>
		/// <remarks>
		/// Setting the value of this property will change <see cref="ActiveItem"/>.
		/// </remarks>
		public IReactiveProperty<int> ActiveItemIndex { get; }

		/// <summary>
		/// Creates a new instance of <see cref="ConductorOneActive{T}"/>.
		/// </summary>
		public ConductorOneActive()
			:
			base(lifetimeChainOptions: new LifetimeChainOptions(activateChildrenOnSelfActivation: false))
		{
			ActiveItem = new ReactiveProperty<T?>(default);
			ActiveItemIndex = new ReactiveProperty<int>(-1);

			_activeItemStack = [];

			ActiveItem.Subscribe(_ =>
			{
				ActivateItemAlreadySet();
			});

			ActiveItemIndex.Subscribe(x =>
			{
				ActiveItem.Value = x < 0 || x >= Items.Count
					? null
					: Items[x];
			});

			Lifetime.Activation.Subscribe(ActivateItemIfNotActive);
		}

		private void ActivateItemIfNotActive()
		{
			ILifetime? lifetime = ActiveItem.Value?.Lifetime;

			if (lifetime == null)
			{
				return;
			}

			if (lifetime.IsActive.Value || lifetime.IsActivating.Value)
			{
				return;
			}

			lifetime.Activate();
		}
		
		private void ActivateItemAlreadySet()
		{
			T? newActiveItem = ActiveItem.Value;

			int index = newActiveItem != null
				? Items.IndexOf(newActiveItem)
				: -1;

			if (index < 0 && newActiveItem != null)
			{
				throw new ArgumentException("Item is not a child of this object.");
			}

			// Last element is the current active item.
			T? currentActiveItem = _activeItemStack.Count == 0
				? null
				: _activeItemStack[_activeItemStack.Count - 1];

			if (newActiveItem == null && currentActiveItem == null)
			{
				// Nothing to do.
				return;
			}

			// We will deactivate the current active item if it is not null.
			currentActiveItem?.Lifetime.Deactivate();

			// If new active item is null, we will update the index property and return.
			if (newActiveItem == null)
			{
				ActiveItemIndex.Value = -1;
				return;
			}

			// New active item is not null. Add it to the active items stack and activate its lifetime.
			PushToActiveItemStack(newActiveItem);
			ActivateItemIfPossible(newActiveItem);

			// Update the index property. It will not call the activate method again since equality comparer
			// will shortcut it.
			ActiveItemIndex.Value = index;
		}

		/// <inheritdoc cref="ConductorBase{T}.Close"/>
		/// <remarks>
		/// If the given item is the active item and there was another item that was
		/// active prior to this item, that previous item is activated.
		/// </remarks>
		public override void Close(
			T item,
			bool result = false,
			CancellationToken cancellationToken = default)
		{
			if (!Items.Contains(item))
			{
				throw new ArgumentException("Item is not a child of this object.");
			}

			bool isActiveItem = IsActiveItem(item);

			ClosureContext context = new(result);

			item.Lifetime.Close(cancellationToken, context);

			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}

			if (context.IsCancelled)
			{
				return;
			}

			RemoveFromActiveItemStack(item);
			// Remove the item from list at the end to prevent external listeners to act on
			// notifications and set the active item to null.

			// Active item is not changing, just return.
			if (!isActiveItem)
			{
				ItemsSource.Remove(item);
				return;
			}

			// The last element will be the previous active element as we have removed current item from stack.
			T? previousActiveItem = _activeItemStack.Count == 0
				? null
				: _activeItemStack[_activeItemStack.Count - 1];

			// We are closing the active item, we should activate the item that was previously active.
			ActiveItem.Value = previousActiveItem;

			ItemsSource.Remove(item);
		}
		
		private void ActivateItemIfPossible(T item)
		{
			bool isActivating = Lifetime.IsActivating.Value;
			bool isActive = Lifetime.IsActive.Value;

			if (!(isActivating || isActive))
			{
				return;
			}

			item.Lifetime.Activate();
		}

		private void PushToActiveItemStack(T item)
		{
			// Remove it first in case it is already in stack.
			_activeItemStack.Remove(item);
			_activeItemStack.Add(item);
		}

		private void RemoveFromActiveItemStack(T item)
		{
			_activeItemStack.Remove(item);
		}
		
		private bool IsActiveItem(T item)
		{
			// Don't use equality operator as it returns wrong values.
			return Equals(item, ActiveItem.Value);
		}
	}
}