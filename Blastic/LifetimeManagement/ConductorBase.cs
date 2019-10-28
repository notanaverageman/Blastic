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
	public class ConductorBase : Screen
	{
		private readonly Dictionary<IHasLifetime, IDisposable> _lifetimeSubscriptions;

		public ReactiveCollection<IHasLifetime> Items { get; }

		public ConductorOptions ConductorOptions { get; }
		public LifetimeChainOptions LifetimeChainOptions { get; }

		public ConductorBase(ExecutionContextFactory executionContextFactory) : base(executionContextFactory)
		{
			_lifetimeSubscriptions = new Dictionary<IHasLifetime, IDisposable>();

			Items = new ReactiveCollection<IHasLifetime>();

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

			void AddChildren(IEnumerable<IHasLifetime> items)
			{
				foreach (IHasLifetime item in items)
				{
					if (_lifetimeSubscriptions.ContainsKey(item))
					{
						continue;
					}

					_lifetimeSubscriptions[item] = Lifetime.AddChildLifetime(item.Lifetime, LifetimeChainOptions);
				}
			}

			void RemoveChildren(IEnumerable<IHasLifetime> items)
			{
				foreach (IHasLifetime item in items)
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