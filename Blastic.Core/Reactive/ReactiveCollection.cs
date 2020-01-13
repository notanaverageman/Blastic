using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Blastic.Platform;

namespace Blastic.Reactive
{
	public class ReactiveCollection<T> : ObservableCollection<T>
	{
		private bool _isNotifying;

		public ReactiveCollection()
		{
			_isNotifying = true;
		}

		public ReactiveCollection(IEnumerable<T> items) : base(items)
		{
			_isNotifying = true;
		}

		protected override void InsertItem(int index, T item) => OnUIThread(() => InsertItemBase(index, item));
		protected virtual void InsertItemBase(int index, T item) => base.InsertItem(index, item);

		protected override void SetItem(int index, T item) => OnUIThread(() => SetItemBase(index, item));
		protected virtual void SetItemBase(int index, T item) => base.SetItem(index, item);

		protected override void RemoveItem(int index) => OnUIThread(() => RemoveItemBase(index));
		protected virtual void RemoveItemBase(int index) => base.RemoveItem(index);

		protected override void ClearItems() => OnUIThread(ClearItemsBase);
		protected virtual void ClearItemsBase() => base.ClearItems();

		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (_isNotifying)
			{
				base.OnCollectionChanged(e);
			}
		}

		protected override void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (_isNotifying)
			{
				base.OnPropertyChanged(e);
			}
		}

		public virtual void AddRange(IEnumerable<T> items)
		{
			void AddRange()
			{
				int index = Count;

				bool previousNotificationSetting = _isNotifying;
				_isNotifying = false;

				foreach (T item in items)
				{
					InsertItemBase(index, item);
					index++;
				}

				_isNotifying = previousNotificationSetting;

				OnPropertyChanged(new PropertyChangedEventArgs("Count"));
				OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}

			OnUIThread(AddRange);
		}

		public virtual void RemoveRange(IEnumerable<T> items)
		{
			void RemoveRange()
			{
				bool previousNotificationSetting = _isNotifying;
				_isNotifying = false;

				foreach (T item in items)
				{
					int index = IndexOf(item);
					if (index >= 0)
					{
						RemoveItemBase(index);
					}
				}

				_isNotifying = previousNotificationSetting;

				OnPropertyChanged(new PropertyChangedEventArgs("Count"));
				OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}

			OnUIThread(RemoveRange);
		}

		private void OnUIThread(Action action)
		{
			PlatformSpecifics.Current.OnUIThread(action);
		}
	}

	public static class ReactiveCollectionExtensions
	{
		public static IObservable<NotifyCollectionChangedEventArgs> CollectionChangedAsObservable<T>(this T source) where T : INotifyCollectionChanged
		{
			return Observable.FromEvent<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
				h => (sender, e) => h(e),
				h => source.CollectionChanged += h,
				h => source.CollectionChanged -= h);
		}

		public static IObservable<T[]> ObserveAdd<T>(this INotifyCollectionChanged source)
		{
			return source.CollectionChangedAsObservable()
				.Where(e => e.Action == NotifyCollectionChangedAction.Add)
				.Select(e => e.NewItems.Cast<T>().ToArray());
		}

		public static IObservable<T[]> ObserveRemove<T>(this INotifyCollectionChanged source)
		{
			return source.CollectionChangedAsObservable()
				.Where(e => e.Action == NotifyCollectionChangedAction.Remove)
				.Select(e => e.OldItems.Cast<T>().ToArray());
		}

		public static IObservable<(T[] OldItems, T[] NewItems)> ObserveMove<T>(this INotifyCollectionChanged source)
		{
			return source.CollectionChangedAsObservable()
				.Where(e => e.Action == NotifyCollectionChangedAction.Move)
				.Select(e => (e.OldItems.Cast<T>().ToArray(), e.NewItems.Cast<T>().ToArray()));
		}

		public static IObservable<(T[] OldItems, T[] NewItems)> ObserveReplace<T>(this INotifyCollectionChanged source)
		{
			return source.CollectionChangedAsObservable()
				.Where(e => e.Action == NotifyCollectionChangedAction.Replace)
				.Select(e => (e.OldItems.Cast<T>().ToArray(), e.NewItems.Cast<T>().ToArray()));
		}

		public static IObservable<Unit> ObserveReset(this INotifyCollectionChanged source)
		{
			return source.CollectionChangedAsObservable()
				.Where(e => e.Action == NotifyCollectionChangedAction.Reset)
				.Select(_ => new Unit());
		}
	}
}