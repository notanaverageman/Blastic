using System.Collections.Generic;
using System.Diagnostics;

namespace Blastic.Reactive
{
	/// <summary>
	/// Default implementation of <see cref="IReactiveProperty{T}"/>.
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	[DebuggerDisplay("{" + nameof(Value) + "}")]
	public class ReactiveProperty<T> : ReactivePropertyBase<T>, IReactiveProperty<T>
	{
		/// <inheritdoc cref="IReactiveProperty{T}.Value"/>
		public T Value
		{
			get => GetValue();
			set => SetValue(value);
		}

		/// <inheritdoc />
		object? IReadOnlyReactiveProperty.Value => Value;

		/// <inheritdoc />
		object? IReactiveProperty.Value
		{
			get => Value;
			set => Value = (T)value!;
		}

		/// <summary>
		/// Creates a new instance with an optional initial value and an optional equality comparer.
		/// </summary>
		/// <param name="initialValue">Initial value of this property.</param>
		/// <param name="equalityComparer">Equality comparer for the values.</param>
		public ReactiveProperty(
			T initialValue,
			IEqualityComparer<T>? equalityComparer = null)
			:
			base(initialValue, equalityComparer)
		{
		}
	}
}