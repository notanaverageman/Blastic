using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blastic.Common;
using Blastic.Execution;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

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

			Items
				.ObserveAddChangedItems()
				.Subscribe(AddChildren);

			Items
				.ObserveRemoveChangedItems()
				.Subscribe(RemoveChildren);

			Items
				.ObserveResetChanged()
				.Subscribe(_ => RemoveChildren(_lifetimeSubscriptions.Keys.ToArray()));

			Items
				.ObserveReplaceChangedItems()
				.Subscribe(items =>
				{
					AddChildren(items.NewItem);
					RemoveChildren(items.OldItem);
				});
		}
	}
}