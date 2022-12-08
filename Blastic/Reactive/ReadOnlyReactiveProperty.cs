using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Blastic.Reactive
{
	/// <summary>
	/// Default implementation of <see cref="IReadOnlyReactiveProperty{T}"/>.
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	[DebuggerDisplay("{" + nameof(Value) + "}")]
	public class ReadOnlyReactiveProperty<T> : ReactivePropertyBase<T>, IReadOnlyReactiveProperty<T>, IObserver<T>
	{
		private readonly IDisposable _sourceSubscription;

		/// <inheritdoc/>
		public T Value => GetValue();

		/// <inheritdoc/>
		object? IReadOnlyReactiveProperty.Value => Value;

		/// <summary>
		/// Creates a new instance that listens to the given observable with an optional initial
		/// value and an optional equality comparer.
		/// </summary>
		/// <param name="source">Observable to listen to.</param>
		/// <param name="initialValue">Initial value of this property.</param>
		/// <param name="equalityComparer">Equality comparer for the values.</param>
		public ReadOnlyReactiveProperty(
			IObservable<T> source,
			T initialValue,
			IEqualityComparer<T>? equalityComparer = null)
			:
			base(initialValue, equalityComparer)
		{
			_sourceSubscription = source.Subscribe(this);
		}

		void IObserver<T>.OnCompleted()
		{
		}

		void IObserver<T>.OnError(Exception error)
		{
		}

		void IObserver<T>.OnNext(T value)
		{
			SetValue(value);
		}

		/// <inheritdoc />
		public override void Dispose()
		{
			_sourceSubscription.Dispose();

			base.Dispose();
		}
	}

	public static class ReadOnlyReactiveProperty
	{
		/// <summary>
		/// Creates a new <see cref="IReadOnlyReactiveProperty{T}"/> that listens to the given observable
		/// with an optional initial value and an optional equality comparer.
		/// </summary>
		/// <param name="source">Observable to listen to.</param>
		/// <param name="initialValue">Initial value of this property.</param>
		/// <param name="equalityComparer">Equality comparer for the values.</param>
		public static ReadOnlyReactiveProperty<T> ToReadOnlyReactiveProperty<T>(
			this IObservable<T> source,
			T initialValue,
			IEqualityComparer<T>? equalityComparer = null)
		{
			return new ReadOnlyReactiveProperty<T>(
				source,
				initialValue,
				equalityComparer);
		}

		/// <summary>
		/// Creates a new <see cref="IReadOnlyReactiveProperty{T}"/> that listens to the given observable
		/// with an optional initial value and an optional equality comparer.
		/// </summary>
		/// <param name="source">Observable to listen to.</param>
		/// <param name="equalityComparer">Equality comparer for the values.</param>
		public static ReadOnlyReactiveProperty<T> ToReadOnlyReactiveProperty<T>(
			this IReactiveProperty<T> source,
			IEqualityComparer<T>? equalityComparer = null)
		{
			return new ReadOnlyReactiveProperty<T>(
				source,
				source.Value,
				equalityComparer);
		}
	}
}