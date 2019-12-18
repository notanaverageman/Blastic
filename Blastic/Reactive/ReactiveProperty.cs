using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Blastic.Reactive
{
	public class ReactiveProperty<T> : IReactiveProperty<T>
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private readonly Subject<T> _source;
		private readonly IObservable<T> _observable;

		private T _value;

		public T Value
		{
			get => _value;
			set => _source.OnNext(value);
		}

		object IReadOnlyReactiveProperty.Value => Value;
		object IReactiveProperty.Value
		{
			get => Value;
			set => Value = (T)value;
		}

		public ReactiveProperty(
			T initialValue = default,
			IEqualityComparer<T> equalityComparer = null)
		{
			_value = initialValue;

			equalityComparer ??= EqualityComparer<T>.Default;

			_source = new Subject<T>();

            _observable = _source.DistinctUntilChanged(equalityComparer);

            _observable
				.Do(x =>
				{
					_value = x;
					PropertyChanged?.Invoke(this, Singletons.PropertyChangedEventArgs);
				})
                .Subscribe();
		}

		public IDisposable Subscribe(IObserver<T> observer)
		{
			return Subscribe(observer, true);
		}

		public IDisposable Subscribe(IObserver<T> observer, bool raiseLatestValue)
		{
			IDisposable disposable = _observable.Subscribe(observer);

			if (raiseLatestValue)
			{
				observer.OnNext(Value);
			}

			return disposable;
		}

		public void Dispose()
		{
			_source?.Dispose();
		}
	}
}