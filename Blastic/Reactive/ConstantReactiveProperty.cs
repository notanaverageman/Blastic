using System;
using System.Collections;
using System.ComponentModel;
using System.Reactive.Disposables;

namespace Blastic.Reactive
{
	/// <summary>
	/// A reactive property that has a constant value. (Not reactive actually.)
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	public class ConstantReactiveProperty<T> : IReadOnlyReactiveProperty<T>
	{
		/// <inheritdoc cref="IReactiveProperty{T}.Value"/>
		public T Value { get; }

		/// <inheritdoc />
		object? IReadOnlyReactiveProperty.Value => Value;

		/// <inheritdoc/>
		public event PropertyChangedEventHandler? PropertyChanged;

		/// <inheritdoc/>
		public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

		/// <inheritdoc/>
		public bool HasErrors => false;

		/// <inheritdoc cref="IReadOnlyReactiveProperty.HasErrorObservable" />
		public IObservable<bool>? HasErrorObservable => null;

		/// <summary>
		/// Create a new instance with given value.
		/// </summary>
		/// <param name="value">Value of the property</param>
		public ConstantReactiveProperty(T value)
		{
			Value = value;
		}

		/// <inheritdoc cref="IObservable{T}.Subscribe"/>
		public IDisposable Subscribe(IObserver<T> observer)
		{
			return Subscribe(observer, true);
		}

		/// <inheritdoc cref="IReadOnlyReactiveProperty{Object}.Subscribe(IObserver{Object},bool)"/>
		public IDisposable Subscribe(IObserver<object?> observer, bool raiseLatestValue)
		{
			if (raiseLatestValue)
			{
				observer.OnNext(Value);
			}

			return Disposable.Empty;
		}

		/// <inheritdoc cref="IReadOnlyReactiveProperty{T}.Subscribe(IObserver{T},bool)"/>
		public IDisposable Subscribe(IObserver<T> observer, bool raiseLatestValue)
		{
			if (raiseLatestValue)
			{
				observer.OnNext(Value);
			}

			return Disposable.Empty;
		}

		/// <inheritdoc cref="IReadOnlyReactiveProperty{T}.AddValidator(System.Func{T,string?})"/>
		public void AddValidator(Func<T, string?> validator)
		{
		}

		/// <inheritdoc cref="IReadOnlyReactiveProperty{T}.AddValidator(System.Func{T,IReadOnlyReactiveProperty{string}?})"/>
		public void AddValidator(Func<T, IReadOnlyReactiveProperty<string>?> validator)
		{
		}

		/// <inheritdoc cref="IReadOnlyReactiveProperty.TriggerValidation"/>
		public void TriggerValidation()
		{
		}

		/// <inheritdoc/>
		public IEnumerable? GetErrors(string propertyName)
		{
			return null;
		}
		
		/// <summary>
		/// Does nothing.
		/// </summary>
		public void Dispose()
		{
		}
	}
}