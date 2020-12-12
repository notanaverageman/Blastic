using System;
using System.Collections.Generic;
using Blastic.Ordering;
using Blastic.Reactive;

namespace Blastic.LifetimeManagement
{
	/// <summary>
	/// A class with a lifecycle that can have multiple child objects whose lifecycles
	/// are managed by this class.
	/// </summary>
	/// <typeparam name="T">A type with a lifecycle.</typeparam>
	public class ConductorBase<T> : Screen where T : IHasLifetime
	{
		private readonly Dictionary<T, IDisposable> _lifetimeSubscriptions;

		/// <summary>
		/// Children of this object.
		/// </summary>
		public ReactiveCollection<T> Items { get; }

		/// <summary>
		/// Options for managing the children.
		/// </summary>
		public ConductorOptions ConductorOptions { get; }

		/// <summary>
		/// Options for managing the lifecycles of the children.
		/// </summary>
		public LifetimeChainOptions LifetimeChainOptions { get; }

		/// <summary>
		/// Creates a new instance with default options.
		/// </summary>
		/// <param name="conductorOptions">The conductor options.</param>
		/// <param name="lifetimeChainOptions">The lifetime options for children.</param>
		public ConductorBase(
			ConductorOptions? conductorOptions = null,
			LifetimeChainOptions? lifetimeChainOptions = null)
		{
			_lifetimeSubscriptions = new Dictionary<T, IDisposable>();

			Items = new ReactiveCollection<T>();

			ConductorOptions = conductorOptions ?? new ConductorOptions();
			LifetimeChainOptions = lifetimeChainOptions ?? new LifetimeChainOptions();
		}

		/// <summary>
		/// Subscribe to the collection changed events of <see cref="Items"/> to manage
		/// the lifecycles of children.
		/// </summary>
		protected void InitializeChildLifetimeSubscriptions()
		{
			if (ConductorOptions.ClearItemsOnDeinitialize)
			{
				Lifetime.Closure.Subscribe(() =>
				{
					Items.Clear();
				}, Order.AbsoluteMaximum);
			}

			void AddChildren(IEnumerable<T> items)
			{
				foreach (T item in items)
				{
					if (_lifetimeSubscriptions.ContainsKey(item))
					{
						continue;
					}

					_lifetimeSubscriptions[item] = Lifetime.AddChildLifetime(item.Lifetime, LifetimeChainOptions);
				}
			}

			void RemoveChildren(IEnumerable<T> items)
			{
				foreach (T item in items)
				{
					if (!_lifetimeSubscriptions.TryGetValue(item, out IDisposable subscription))
					{
						continue;
					}

					subscription.Dispose();
					_lifetimeSubscriptions.Remove(item);
				}
			}

			void ReplaceChildren((T[] OldItems, T[] NewItems) items)
			{
				RemoveChildren(items.OldItems);
				AddChildren(items.NewItems);
			}

			void ResetChildren()
			{
				RemoveChildren(Items);
				AddChildren(Items);
			}

			Items
				.ObserveAdd<T>()
				.Subscribe(AddChildren);

			Items
				.ObserveRemove<T>()
				.Subscribe(RemoveChildren);

			Items
				.ObserveReplace<T>()
				.Subscribe(ReplaceChildren);

			Items
				.ObserveReset()
				.Subscribe(_ => ResetChildren());
		}
	}
}