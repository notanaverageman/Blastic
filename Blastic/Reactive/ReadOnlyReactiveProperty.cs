using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace Blastic.Reactive
{
	public class ReadOnlyReactiveProperty<T> : ReactivePropertyBase<T>, IReadOnlyReactiveProperty<T>
	{
		protected override IObservable<T> Source { get; }

		public T Value { get; private set; }
		object IReadOnlyReactiveProperty.Value => Value;

		public ReadOnlyReactiveProperty(
			IObservable<T> source,
			T initialValue = default,
			IEqualityComparer<T> equalityComparer = null)
		{
			equalityComparer ??= EqualityComparer<T>.Default;

			Source = source.DistinctUntilChanged(equalityComparer);
			Value = initialValue;

			Initialize();
		}

		protected override T GetValue()
		{
			return Value;
		}

		protected override void SetValue(T value)
		{
			Value = value;
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