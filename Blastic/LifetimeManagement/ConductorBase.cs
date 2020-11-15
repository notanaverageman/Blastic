using System;
using System.Collections.Generic;
using Blastic.Ordering;
using Blastic.Reactive;

namespace Blastic.LifetimeManagement
{
	public class ConductorBase<T> : Screen where T : IHasLifetime
	{
		private readonly Dictionary<T, IDisposable> _lifetimeSubscriptions;

		public ReactiveCollection<T> Items { get; }

		public ConductorOptions ConductorOptions { get; }
		public LifetimeChainOptions LifetimeChainOptions { get; }

		public ConductorBase()
		{
			_lifetimeSubscriptions = new Dictionary<T, IDisposable>();

			Items = new ReactiveCollection<T>();

			ConductorOptions = new ConductorOptions();
			LifetimeChainOptions = new LifetimeChainOptions();
		}

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