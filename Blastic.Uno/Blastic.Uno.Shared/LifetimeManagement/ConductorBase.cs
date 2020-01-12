using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blastic.Common;
using Blastic.Execution;
using Blastic.Reactive;

namespace Blastic.LifetimeManagement
{
	public class ConductorBase<T> : Screen where T : IHasLifetime
	{
		private readonly Dictionary<T, IDisposable> _lifetimeSubscriptions;

		public ReactiveCollection<T> Items { get; }

		public ConductorOptions ConductorOptions { get; }
		public LifetimeChainOptions LifetimeChainOptions { get; }

		public ConductorBase(ExecutionContextFactory executionContextFactory) : base(executionContextFactory)
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
				Lifetime.Close.Subscribe(_ =>
				{
					Items.Clear();
					return Task.CompletedTask;
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
					_lifetimeSubscriptions[item]?.Dispose();
					_lifetimeSubscriptions.Remove(item);
				}
			}

			void ReplaceChildren((T[] OldItems, T[] NewItems) items)
			{
				RemoveChildren(items.OldItems);
				AddChildren(items.NewItems);
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
				.ObserveReset<T>()
				.Subscribe(_ => RemoveChildren(_lifetimeSubscriptions.Keys.ToArray()));
		}
	}
}