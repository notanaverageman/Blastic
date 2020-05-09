using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Blastic.Reactive
{
	public abstract class ReactivePropertyBase<T> : INotifyPropertyChanged, INotifyDataErrorInfo
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

		private readonly Lazy<IObservable<bool>> _hasErrorObservable;
		private readonly List<Func<T, string>> _validators;
		private readonly List<string> _errors;

		protected abstract IObservable<T> Source { get; }

		public bool HasErrors => _errors.Count > 0;
		public IObservable<bool> HasErrorObservable => _hasErrorObservable.Value;

		public ReactivePropertyBase()
		{
			_validators = new List<Func<T, string>>();
			_errors = new List<string>();

			IObservable<bool> CreateHasErrorObservable()
			{
				return Observable.FromEventPattern<DataErrorsChangedEventArgs>(
						x => ErrorsChanged += x,
						x => ErrorsChanged -= x)
					.Select(x => HasErrors);
			}

			_hasErrorObservable = new Lazy<IObservable<bool>>(CreateHasErrorObservable);
		}

		protected void Initialize()
		{
			Source
				.Subscribe(x =>
				{
					SetValue(x);
					TriggerValidation();

					PropertyChanged?.Invoke(this, Singletons.PropertyChangedEventArgs);
				});
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
			_validators.Add(validator);
		}

		public void TriggerValidation()
		{
			_errors.Clear();

			foreach (Func<T, string> validator in _validators)
			{
				string error = validator(GetValue());

				if (string.IsNullOrEmpty(error))
				{
					continue;
				}

				_errors.Add(error);
			}

			ErrorsChanged?.Invoke(this, Singletons.DataErrorsChangedEventArgs);
		}

		public IEnumerable GetErrors(string propertyName)
		{
			return _errors;
		}

		protected abstract T GetValue();
		protected abstract void SetValue(T value);
	}
}