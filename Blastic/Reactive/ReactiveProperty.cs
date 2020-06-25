using System.Collections.Generic;

namespace Blastic.Reactive
{
	public class ReactiveProperty<T> : ReactivePropertyBase<T>, IReactiveProperty<T>
	{
		public T Value
		{
			get => GetValue();
			set => SetValue(value);
		}

		object? IReadOnlyReactiveProperty.Value => Value;
		object? IReactiveProperty.Value
		{
			get => Value;
			set => Value = (T)value;
		}

		public ReactiveProperty(
			T initialValue = default,
			IEqualityComparer<T>? equalityComparer = null)
			:
			base(initialValue, equalityComparer)
		{
		}
	}
}