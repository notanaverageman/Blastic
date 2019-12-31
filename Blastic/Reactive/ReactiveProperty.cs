using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Blastic.Reactive
{
	public class ReactiveProperty<T> : IReactiveProperty<T>
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

		private readonly Subject<T> _source;
		private readonly IObservable<T> _observable;

		private readonly List<Func<T, string>> _validators;

		private T _value;
		private IEnumerable<string> _errors;

		public T Value
		{
			get => _value;
			set => _source.OnNext(value);
		}

		object IReadOnlyReactiveProperty.Value => Value;
		object IReactiveProperty.Value
		{
			get => Value;
			set => Value = (T)value;
		}

		public bool HasErrors => _errors != null;

		public ReactiveProperty(
			T initialValue = default,
			IEqualityComparer<T> equalityComparer = null)
		{
			_value = initialValue;

			equalityComparer ??= EqualityComparer<T>.Default;

			_validators = new List<Func<T, string>>();
			_source = new Subject<T>();

            _observable = _source.DistinctUntilChanged(equalityComparer);

            _observable
				.Do(x =>
				{
					_value = x;

					TriggerValidation();

					PropertyChanged?.Invoke(this, Singletons.PropertyChangedEventArgs);
				})
                .Subscribe();
		}

		public IDisposable Subscribe(IObserver<T> observer)
		{
			return Subscribe(observer, true);
		}

		public IDisposable Subscribe(IObserver<T> observer, bool raiseLatestValue)
		{
			IDisposable disposable = _observable.Subscribe(observer);

			if (raiseLatestValue)
			{
				observer.OnNext(Value);
			}

			return disposable;
		}

		public void AddValidator(Func<T, string> validator)
		{
			_validators.Add(validator);
		}

		public void TriggerValidation()
		{
			_errors = _validators
				.Select(y => y(Value))
				.Where(y => !string.IsNullOrEmpty(y))
				.ToList();

			ErrorsChanged?.Invoke(this, Singletons.DataErrorsChangedEventArgs);
		}

		public void Dispose()
		{
			_source?.Dispose();
		}

		public IEnumerable GetErrors(string propertyName)
		{
			return _errors;
		}
	}
}