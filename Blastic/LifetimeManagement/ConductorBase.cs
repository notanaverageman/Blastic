using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Blastic.Ordering;
using DynamicData;

namespace Blastic.LifetimeManagement
{
	/// <summary>
	/// A class with a lifecycle that can have multiple child objects whose lifecycles
	/// are managed by this class.
	/// </summary>
	/// <typeparam name="T">A type with a lifecycle.</typeparam>
	public class ConductorBase<T> : IHasLifetime where T : IHasLifetime
	{
		private readonly Dictionary<T, IDisposable> _lifetimeSubscriptions;

		/// <inheritdoc />
		public ILifetime Lifetime { get; }

		/// <summary>
		/// Children of this object as <see cref="ISourceList{T}"/>
		/// </summary>
		public ISourceList<T> ItemsSource { get; }

		/// <summary>
		/// Children of this object.
		/// </summary>
		public ReadOnlyObservableCollection<T> Items { get; }

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

			ConductorOptions = conductorOptions ?? new ConductorOptions();
			LifetimeChainOptions = lifetimeChainOptions ?? new LifetimeChainOptions();
			
			Lifetime = new Lifetime();
			ItemsSource = new SourceList<T>();

			ItemsSource
				.Connect()
				.Bind(out ReadOnlyObservableCollection<T> items)
				.DisposeMany()
				.Subscribe(ItemsChanged);

			Items = items;

			if (ConductorOptions.ClearItemsOnDeinitialize)
			{
				Lifetime.Closure.Subscribe(() =>
				{
					ItemsSource.Clear();
				}, Order.AbsoluteMaximum);
			}
		}

		private void ItemsChanged(IChangeSet<T> changeSet)
		{
			void HandleAdd(T item)
			{
				if (_lifetimeSubscriptions.ContainsKey(item))
				{
					return;
				}

				_lifetimeSubscriptions[item] = Lifetime.AddChildLifetime(item.Lifetime, LifetimeChainOptions);
			}

			void HandleRemove(T item)
			{
				if (!_lifetimeSubscriptions.TryGetValue(item, out IDisposable subscription))
				{
					return;
				}

				subscription.Dispose();
				_lifetimeSubscriptions.Remove(item);
			}

			foreach (Change<T> change in changeSet)
			{
				switch (change.Reason)
				{
					case ListChangeReason.Add:
						HandleAdd(change.Item.Current);
						break;

					case ListChangeReason.AddRange:
						foreach (T item in change.Range)
						{
							HandleAdd(item);
						}
						break;

					case ListChangeReason.Remove:
						HandleRemove(change.Item.Current);
						break;

					case ListChangeReason.RemoveRange:
					case ListChangeReason.Clear:
						foreach (T item in change.Range)
						{
							HandleRemove(item);
						}
						break;
					
					case ListChangeReason.Replace:
						HandleRemove(change.Item.Previous.Value);
						HandleAdd(change.Item.Current);
						break;
				}
			}
		}
	}
}