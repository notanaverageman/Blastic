using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Blastic.Ordering;
using DynamicData;

namespace Blastic.LifetimeManagement;

public abstract class ConductorBaseCommon<T>
{
	private readonly Dictionary<T, IDisposable> _lifetimeSubscriptions;
	private readonly SourceListWithListMethods _itemsSource;

	/// <summary>
	/// Children of this object as <see cref="ISourceList{T}"/>
	/// </summary>
	public ISourceList<T> ItemsSource => _itemsSource;

	/// <summary>
	/// Children of this object.
	/// </summary>
	public ReadOnlyObservableCollection<T> Items => _itemsSource.ReadOnlyObservableCollection;

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
	public ConductorBaseCommon(
		ConductorOptions? conductorOptions,
		LifetimeChainOptions? lifetimeChainOptions)
	{
		_lifetimeSubscriptions = new Dictionary<T, IDisposable>();
		_itemsSource = new SourceListWithListMethods();

		_itemsSource.Connect()
			.OnItemAdded(HandleAdd)
			.OnItemRemoved(HandleRemove)
			.Subscribe();

		ConductorOptions = conductorOptions ?? new ConductorOptions();
		LifetimeChainOptions = lifetimeChainOptions ?? new LifetimeChainOptions();
	}

	protected void SubscribeToLifetimeClosure(ILifetime lifetime)
	{
		if (ConductorOptions.ClearItemsOnClosure)
		{
			lifetime.Closure.Subscribe(() =>
			{
				_itemsSource.Clear();
			}, Order.AbsoluteMaximum);
		}
	}

	protected void SubscribeToAsyncLifetimeClosure(IAsyncLifetime lifetime)
	{
		if (ConductorOptions.ClearItemsOnClosure)
		{
			lifetime.Closure.Subscribe(() =>
			{
				_itemsSource.Clear();
			}, Order.AbsoluteMaximum);
		}
	}

	protected abstract IDisposable AddChildLifetime(T item);

	private void HandleAdd(T item)
	{
		if (_lifetimeSubscriptions.ContainsKey(item))
		{
			return;
		}

		_lifetimeSubscriptions[item] = AddChildLifetime(item);
	}

	private void HandleRemove(T item)
	{
		if (!_lifetimeSubscriptions.TryGetValue(item, out IDisposable subscription))
		{
			return;
		}

		subscription.Dispose();
		_lifetimeSubscriptions.Remove(item);
	}
	
	private class SourceListWithListMethods : ISourceList<T>, IList<T>, INotifyCollectionChanged, INotifyPropertyChanged
	{
		private readonly SourceList<T> _sourceList;
		
		public bool IsReadOnly => false;
		public int Count => _sourceList.Count;
		public IObservable<int> CountChanged => _sourceList.CountChanged;
		public IEnumerable<T> Items => _sourceList.Items;
		public ReadOnlyObservableCollection<T> ReadOnlyObservableCollection { get; }

		public SourceListWithListMethods()
		{
			_sourceList = new SourceList<T>();
			_sourceList
				.Connect()
				.Bind(out ReadOnlyObservableCollection<T> x)
				.Subscribe();

			ReadOnlyObservableCollection = x;
		}

		public void Edit(Action<IExtendedList<T>> updateAction)
		{
			_sourceList.Edit(updateAction);
		}

		public IObservable<IChangeSet<T>> Connect(Func<T, bool>? predicate = null)
		{
			return _sourceList.Connect(predicate);
		}

		public IObservable<IChangeSet<T>> Preview(Func<T, bool>? predicate = null)
		{
			return _sourceList.Preview(predicate);
		}

		public void Dispose()
		{
			_sourceList.Dispose();
		}
		
		public void Add(T item)
		{
			_sourceList.Add(item);
		}

		public void Clear()
		{
			_sourceList.Clear();
		}

		public bool Contains(T item)
		{
			return ReadOnlyObservableCollection.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			ReadOnlyObservableCollection.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item)
		{
			return _sourceList.Remove(item);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return ReadOnlyObservableCollection.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ReadOnlyObservableCollection.GetEnumerator();
		}

		public int IndexOf(T item)
		{
			return ReadOnlyObservableCollection.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			_sourceList.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			_sourceList.RemoveAt(index);
		}

		public T this[int index]
		{
			get => ReadOnlyObservableCollection[index];
			set => _sourceList.ReplaceAt(index, value);
		}

		public event NotifyCollectionChangedEventHandler? CollectionChanged
		{
			add => ((INotifyCollectionChanged)ReadOnlyObservableCollection).CollectionChanged += value;
			remove => ((INotifyCollectionChanged)ReadOnlyObservableCollection).CollectionChanged -= value;
		}

		public event PropertyChangedEventHandler? PropertyChanged
		{
			add => ((INotifyPropertyChanged)ReadOnlyObservableCollection).PropertyChanged += value;
			remove => ((INotifyPropertyChanged)ReadOnlyObservableCollection).PropertyChanged -= value;
		}
	}
}