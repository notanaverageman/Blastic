using System;
using System.ComponentModel;

namespace Blastic.Reactive
{
	/// <summary>
	/// This interface provides the current value and raises events on property change
	/// and data errors.
	/// </summary>
	public interface IReadOnlyReactiveProperty : INotifyPropertyChanged, INotifyDataErrorInfo
	{
		/// <summary>
		/// Current value of the property.
		/// </summary>
		object? Value { get; }

		/// <summary>
		/// Force trigger a new validation on current value.
		/// </summary>
		void TriggerValidation();

		/// <summary>
		/// An observable that emits true when there is a data error.
		/// </summary>
		/// <remarks>
		/// This property will be null if there is no validation logic registered
		/// with the property.
		/// </remarks>
		IObservable<bool>? HasErrorObservable { get; }

		/// <summary>
		/// Subscribe to this observable. if <see cref="raiseLatestValue"/> is true, current value
		/// will be emitted to the given observer immediately.
		/// </summary>
		/// <param name="observer">The observer that will observe this property.</param>
		/// <param name="raiseLatestValue">Whether to emit the current value upon subscription.</param>
		/// <returns>A disposable that unsubscribes the observer upon disposal.</returns>
		IDisposable Subscribe(IObserver<object?> observer, bool raiseLatestValue);
	}

	/// <summary>
	/// This interface extends <see cref="IReadOnlyReactiveProperty"/> and <see cref="IObservable{T}"/>
	/// and defines the <see cref="Value"/> as a generic property.
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	public interface IReadOnlyReactiveProperty<out T> : IReadOnlyReactiveProperty, IObservable<T>
	{
		/// <inheritdoc cref="IReadOnlyReactiveProperty.Value"/>
		new T Value { get; }

		/// <summary>
		/// Subscribe to this observable. if <see cref="raiseLatestValue"/> is true, current value
		/// will be emitted to the given observer immediately.
		/// </summary>
		/// <remarks>
		/// Subscribing via <see cref="IObservable{T}.Subscribe"/> method will emit the current value
		/// immediately.
		/// </remarks>
		/// <param name="observer">The observer that will observe this property.</param>
		/// <param name="raiseLatestValue">Whether to emit the current value upon subscription.</param>
		/// <returns>A disposable that unsubscribes the observer upon disposal.</returns>
		IDisposable Subscribe(IObserver<T> observer, bool raiseLatestValue);

		/// <summary>
		/// Add a validator function to this property.
		/// </summary>
		/// <param name="validator">
		/// A function that returns a non-null, non-empty error message if current value is not valid.
		/// </param>
		void AddValidator(Func<T, string?> validator);

		/// <summary>
		/// Add a validator function to this property.
		/// </summary>
		/// <param name="validator">
		/// A function that returns a non-null observable property for error message if current value is not valid.
		/// </param>
		void AddValidator(Func<T, IReadOnlyReactiveProperty<string>?> validator);
	}
}