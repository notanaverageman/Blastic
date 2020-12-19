using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Subjects;

namespace Blastic.Reactive
{
	/// <summary>
	/// Abstract base class that implements the core logic for reactive properties.
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	public abstract class ReactivePropertyBase<T> : INotifyPropertyChanged, INotifyDataErrorInfo
	{
		/// <inheritdoc/>
		public event PropertyChangedEventHandler? PropertyChanged;

		/// <inheritdoc/>
		public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

		private readonly IEqualityComparer<T>? _equalityComparer;
		private readonly Subject<T> _source;

		private ReactivePropertyErrorHandler<T>? _errorHandler;
		private T _value;

		/// The source observable for this property's value.
		protected IObservable<T> Source => _source;

		/// <inheritdoc/>
		public bool HasErrors => _errorHandler?.HasErrors == true;

		/// <inheritdoc cref="IReadOnlyReactiveProperty.HasErrorObservable" />
		public IObservable<bool>? HasErrorObservable => _errorHandler?.HasErrorObservable;

		/// <summary>
		/// Create a new instance with given initial value and an optional equality comparer.
		/// </summary>
		/// <param name="initialValue">Initial value of the property</param>
		/// <param name="equalityComparer">Equality comparer for the values.</param>
		protected ReactivePropertyBase(
			T initialValue,
			IEqualityComparer<T>? equalityComparer)
		{
			_value = initialValue;
			_equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;

			_source = new Subject<T>();
		}

		/// <inheritdoc cref="IObservable{T}.Subscribe"/>
		public IDisposable Subscribe(IObserver<T> observer)
		{
			return Subscribe(observer, true);
		}

		/// <inheritdoc cref="IReadOnlyReactiveProperty{T}.Subscribe(IObserver{T},bool)"/>
		public IDisposable Subscribe(IObserver<T> observer, bool raiseLatestValue)
		{
			IDisposable disposable = Source.Subscribe(observer);

			if (raiseLatestValue)
			{
				observer.OnNext(GetValue());
			}

			return disposable;
		}

		/// <inheritdoc cref="IReadOnlyReactiveProperty{T}.AddValidator(System.Func{T,string?})"/>
		public void AddValidator(Func<T, string?> validator)
		{
			_errorHandler ??= new ReactivePropertyErrorHandler<T>(this);
			_errorHandler.AddValidator(validator);
		}

		/// <inheritdoc cref="IReadOnlyReactiveProperty{T}.AddValidator(System.Func{T,IReadOnlyReactiveProperty{string}?})"/>
		public void AddValidator(Func<T, IReadOnlyReactiveProperty<string>?> validator)
		{
			_errorHandler ??= new ReactivePropertyErrorHandler<T>(this);
			_errorHandler.AddValidator(validator);
		}

		/// <inheritdoc cref="IReadOnlyReactiveProperty.TriggerValidation"/>
		public void TriggerValidation()
		{
			_errorHandler?.TriggerValidation(_value);
		}

		/// <inheritdoc/>
		public IEnumerable? GetErrors(string propertyName)
		{
			return _errorHandler?.Errors;
		}

		internal void InvokeErrorsChanged()
		{
			ErrorsChanged?.Invoke(this, Singletons.DataErrorsChangedEventArgs);
		}

		/// <summary>
		/// Returns the current value.
		/// </summary>
		/// <returns></returns>
		protected T GetValue()
		{
			return _value;
		}

		/// <summary>
		/// Sets the given value if it is not equal to current value, triggers a validation, and raises
		/// a property changed event.
		/// </summary>
		/// <param name="value"></param>
		protected void SetValue(T value)
		{
			if (_equalityComparer?.Equals(_value, value) == true)
			{
				return;
			}

			_value = value;
			TriggerValidation();

			PropertyChanged?.Invoke(this, Singletons.PropertyChangedEventArgs);
			_source.OnNext(value);
		}

		/// <summary>
		/// Completes this observable and disposes the underlying source.
		/// </summary>
		public virtual void Dispose()
		{
			_source.OnCompleted();
			_source.Dispose();
		}
	}
}