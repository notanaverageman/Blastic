using System;
using System.Reactive.Linq;
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
		private readonly IReadOnlyReactiveProperty<T?> _previousActiveItem;

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
			ActiveItem = new ReactiveProperty<T?>();
			ActiveItemIndex = new ReactiveProperty<int>();

			_previousActiveItem = ActiveItem
				.WithPrevious()
				.Select(x => x.Previous)
				.ToReadOnlyReactiveProperty();

			ActiveItem.Subscribe(x =>
			{
				Activate(x);
			});

			ActiveItemIndex.Subscribe(x =>
			{
				if (x < 0 || x >= Items.Count)
				{
					Activate(null);
				}
				else
				{
					Activate(Items[x]);
				}
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

		/// <summary>
		/// Activate the given item. Adds the item to children if it is not added before
		/// and the given item is not null.
		/// </summary>
		/// <remarks>
		/// This deactivates the currently active item if there is one.
		/// </remarks>
		/// <param name="item">Item to activate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		public void Activate(T? item, CancellationToken cancellationToken = default)
		{
			int index = item != null ? Items.IndexOf(item) : -1;

			if (item != null && index < 0)
			{
				ItemsSource.Add(item);
				index = Items.Count - 1;
			}

			// This does not cause a stack overflow since equality comparer in ActiveItem.Value
			// returns early if we set the same item.
			ActiveItem.Value = item;
			ActiveItemIndex.Value = index;

			bool isActivating = Lifetime.IsActivating.Value;
			bool isActive = Lifetime.IsActive.Value;

			if (!(isActivating || isActive))
			{
				return;
			}

			ChangeActiveItem(cancellationToken);
		}

		/// <summary>
		/// Close the given item and remove it from children.
		/// </summary>
		/// <remarks>
		/// If the given item is the active item and there was another item that was
		/// active prior to this item, that previous item is activated.
		/// </remarks>
		/// <param name="item">The item to close.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="result">The result of the closure operation.</param>
		public void Close(
			T item,
			CancellationToken cancellationToken = default,
			bool result = false)
		{
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

			if (Equals(item, ActiveItem.Value))
			{
				ActiveItem.Value = _previousActiveItem.Value;
			}

			ItemsSource.Remove(item);
		}

		private void ChangeActiveItem(CancellationToken cancellationToken)
		{
			IHasLifetime? previousActiveItem = _previousActiveItem.Value;
			IHasLifetime? activeItem = ActiveItem.Value;

			if (Equals(previousActiveItem, activeItem))
			{
				return;
			}

			previousActiveItem?.Lifetime.Deactivate(cancellationToken);
			activeItem?.Lifetime.Activate(cancellationToken);
		}
	}
}