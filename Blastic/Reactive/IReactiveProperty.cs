using System;

namespace Blastic.Reactive
{
	/// <summary>
	/// Inherits from <see cref="IReadOnlyReactiveProperty"/> and provides a setter
	/// for <see cref="Value"/>.
	/// </summary>
	public interface IReactiveProperty : IReadOnlyReactiveProperty, IDisposable
	{
		/// <inheritdoc cref="IReadOnlyReactiveProperty.Value"/>
		new object? Value { get; set; }
	}

	/// <summary>
	/// Inherits from <see cref="IReadOnlyReactiveProperty{T}"/> and provides a setter
	/// for <see cref="Value"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IReactiveProperty<T> : IReactiveProperty, IReadOnlyReactiveProperty<T>
	{
		/// <inheritdoc cref="IReactiveProperty.Value"/>
		new T Value { get; set; }
	}
}