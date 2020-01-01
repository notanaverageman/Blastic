using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Blastic.Reactive
{
	public class ReadOnlyReactiveProperty<T> : IReadOnlyReactiveProperty<T>
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

		private readonly List<string> _errors;

		private readonly IObservable<T> _source;
		private readonly List<Func<T, string>> _validators;

		public T Value { get; private set; }
		object IReadOnlyReactiveProperty.Value => Value;

		public bool HasErrors => _errors?.Any() == true;
		public IObservable<bool> HasErrorObservable { get; }

		public ReadOnlyReactiveProperty(
			IObservable<T> source,
			T initialValue = default,
			IEqualityComparer<T> equalityComparer = null)
		{
			equalityComparer ??= EqualityComparer<T>.Default;

			Value = initialValue;

			_validators = new List<Func<T, string>>();
			_errors = new List<string>();

			_source = source.DistinctUntilChanged(equalityComparer);

			_source.Subscribe(x =>
			{
				Value = x;

				TriggerValidation();

				PropertyChanged?.Invoke(this, Singletons.PropertyChangedEventArgs);
			});

			HasErrorObservable = Observable.FromEventPattern<DataErrorsChangedEventArgs>(
					x => ErrorsChanged += x,
					x => ErrorsChanged -= x)
				.Select(x => HasErrors);
		}

		public IDisposable Subscribe(IObserver<T> observer)
		{
			return Subscribe(observer, true);
		}

		public IDisposable Subscribe(IObserver<T> observer, bool raiseLatestValue)
		{
			IDisposable disposable = _source.Subscribe(observer);

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

		public IEnumerable GetErrors(string propertyName)
		{
			return _errors;
		}

		public void TriggerValidation()
		{
			_errors.Clear();

			foreach (Func<T, string> validator in _validators)
			{
				string error = validator(Value);

				if (string.IsNullOrEmpty(error))
				{
					continue;
				}

				_errors.Add(error);
			}

			ErrorsChanged?.Invoke(this, Singletons.DataErrorsChangedEventArgs);
		}
	}

	public static class ReadOnlyReactiveProperty
	{
		public static ReadOnlyReactiveProperty<T> ToReadOnlyReactiveProperty<T>(
			this IObservable<T> source,
			T initialValue = default,
			IEqualityComparer<T> equalityComparer = null)
		{
			return new ReadOnlyReactiveProperty<T>(
				source,
				initialValue,
				equalityComparer);
		}
	}
}