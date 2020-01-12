using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Blastic.Reactive
{
	public class ReactiveProperty<T> : ReactivePropertyBase<T>, IReactiveProperty<T>
	{
		private readonly Subject<T> _source;

		private T _value;

		protected override IObservable<T> Source { get; }

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
			_source = new Subject<T>();

			equalityComparer ??= EqualityComparer<T>.Default;

			Source = _source.DistinctUntilChanged(equalityComparer);

			Initialize();
		}

		public void Dispose()
		{
			_source.OnCompleted();
			_source.Dispose();
		}

		protected override T GetValue()
		{
			return Value;
		}

		protected override void SetValue(T value)
		{
			_value = value;
		}
	}
}