using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Blastic.Reactive
{
	[DebuggerDisplay("{" + nameof(Value) + "}")]
	public class ReadOnlyReactiveProperty<T> : ReactivePropertyBase<T>, IReadOnlyReactiveProperty<T>
	{
		private readonly IDisposable _sourceSubscription;

		public T Value => GetValue();

		object? IReadOnlyReactiveProperty.Value => Value;

		public ReadOnlyReactiveProperty(
			IObservable<T> source,
			T initialValue = default,
			IEqualityComparer<T>? equalityComparer = null)
			:
			base(initialValue, equalityComparer)
		{
			_sourceSubscription = source.Subscribe(OnNext);
		}

		private void OnNext(T value)
		{
			SetValue(value);
		}

		public override void Dispose()
		{
			_sourceSubscription.Dispose();

			base.Dispose();
		}
	}

	public static class ReadOnlyReactiveProperty
	{
		public static ReadOnlyReactiveProperty<T> ToReadOnlyReactiveProperty<T>(
			this IObservable<T> source,
			T initialValue = default,
			IEqualityComparer<T>? equalityComparer = null)
		{
			return new ReadOnlyReactiveProperty<T>(
				source,
				initialValue,
				equalityComparer);
		}
	}
}