using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Blastic.Reactive
{
	public class ReadOnlyReactiveProperty<T> : IReadOnlyReactiveProperty<T>
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private readonly IObservable<T> _source;

		public T Value { get; private set; }
		object IReadOnlyReactiveProperty.Value => Value;

		public ReadOnlyReactiveProperty(
			IObservable<T> source,
			T initialValue = default,
			IEqualityComparer<T> equalityComparer = null)
		{
			equalityComparer ??= EqualityComparer<T>.Default;

			Value = initialValue;

			_source = source
				.DistinctUntilChanged(equalityComparer)
				.Do(x =>
				{
					Value = x;
					PropertyChanged?.Invoke(this, Singletons.PropertyChangedEventArgs);
				});
		}

		public IDisposable Subscribe(IObserver<T> observer)
		{
			return Subscribe(observer, true);
		}

		public IDisposable Subscribe(IObserver<T> observer, bool raiseLatestValue)
		{
			IDisposable disposable = _source.Subscribe(observer);

			if (raiseLatestValue)
			{
				observer.OnNext(Value);
			}

			return disposable;
		}
	}

	public static class ReadOnlyReactiveProperty
	{
		public static ReadOnlyReactiveProperty<T> ToReadOnlyReactiveProperty<T>(
			this IObservable<T> source,
			T initialValue = default,
			IEqualityComparer<T> equalityComparer = null)
		{
			return new ReadOnlyReactiveProperty<T>(
				source,
				initialValue,
				equalityComparer);
		}
	}
}