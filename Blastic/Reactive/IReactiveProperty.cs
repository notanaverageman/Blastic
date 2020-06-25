using System;

namespace Blastic.Reactive
{
	public interface IReactiveProperty : IReadOnlyReactiveProperty, IDisposable
	{
		new object? Value { get; set; }
	}

	public interface IReactiveProperty<T> : IReactiveProperty, IReadOnlyReactiveProperty<T>
	{
		new T Value { get; set; }
	}
}