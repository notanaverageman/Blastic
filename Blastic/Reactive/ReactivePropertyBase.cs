using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Blastic.Reactive
{
	public abstract class ReactivePropertyBase<T> : INotifyPropertyChanged, INotifyDataErrorInfo
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

		private readonly IEqualityComparer<T> _equalityComparer;
		private readonly Subject<T> _source;

		private ReactivePropertyErrorHandler<T> _errorHandler;
		private T _value;

		protected IObservable<T> Source => _source;

		public bool HasErrors => _errorHandler?.HasErrors == true;
		public IObservable<bool> HasErrorObservable => _errorHandler?.HasErrorObservable;

		protected ReactivePropertyBase(
			T initialValue,
			IEqualityComparer<T> equalityComparer)
		{
			_value = initialValue;
			_equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;

			_source = new Subject<T>();
		}

		public IDisposable Subscribe(IObserver<T> observer)
		{
			return Subscribe(observer, true);
		}

		public IDisposable Subscribe(IObserver<T> observer, bool raiseLatestValue)
		{
			IDisposable disposable = Source.Subscribe(observer);

			if (raiseLatestValue)
			{
				observer.OnNext(GetValue());
			}

			return disposable;
		}

		public void AddValidator(Func<T, string> validator)
		{
			_errorHandler ??= new ReactivePropertyErrorHandler<T>(this);
			_errorHandler.AddValidator(validator);
		}

		public void TriggerValidation()
		{
			_errorHandler?.TriggerValidation(_value);
		}

		public IEnumerable GetErrors(string propertyName)
		{
			return _errorHandler?.Errors;
		}

		internal void InvokeErrorsChanged()
		{
			ErrorsChanged?.Invoke(this, Singletons.DataErrorsChangedEventArgs);
		}

		protected T GetValue()
		{
			return _value;
		}

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

		public virtual void Dispose()
		{
			_source.OnCompleted();
			_source.Dispose();
		}
	}
}