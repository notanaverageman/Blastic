using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Blastic.Reactive
{
	internal class ReactivePropertyErrorHandler<T>
	{
		private readonly ReactivePropertyBase<T> _source;
		private readonly List<string> _errors;

		private readonly List<Func<T, string?>> _validators;
		private readonly List<Func<T, IReadOnlyReactiveProperty<string>?>> _reactiveValidators;
		private readonly Dictionary<Func<T, IReadOnlyReactiveProperty<string>?>, IDisposable> _reactiveValidatorSubscriptions;

		public IEnumerable<string> Errors => _errors;

		public bool HasErrors => _errors.Count > 0;
		public IObservable<bool> HasErrorObservable { get; }

		public ReactivePropertyErrorHandler(ReactivePropertyBase<T> source)
		{
			_source = source;
			_errors = new List<string>();

			_validators = new List<Func<T, string?>>();

			_reactiveValidators = new List<Func<T, IReadOnlyReactiveProperty<string>?>>();
			_reactiveValidatorSubscriptions = new Dictionary<Func<T, IReadOnlyReactiveProperty<string>?>, IDisposable>();

			HasErrorObservable = Observable
				.FromEventPattern<DataErrorsChangedEventArgs>(
					x => source.ErrorsChanged += x,
					x => source.ErrorsChanged -= x)
				.Select(_ => HasErrors);
		}

		public void AddValidator(Func<T, string?> validator)
		{
			_validators.Add(validator);
		}

		public void AddValidator(Func<T, IReadOnlyReactiveProperty<string>?> validator)
		{
			_reactiveValidators.Add(validator);
		}

		public void TriggerValidation(T value)
		{
			_errors.Clear();

			foreach (Func<T, string?> validator in _validators)
			{
				string? error = validator(value);

				if (string.IsNullOrEmpty(error))
				{
					continue;
				}

				_errors.Add(error!);
			}

			foreach (IDisposable subscription in _reactiveValidatorSubscriptions.Values)
			{
				subscription.Dispose();
			}

			_reactiveValidatorSubscriptions.Clear();

			foreach (Func<T, IReadOnlyReactiveProperty<string>?> validator in _reactiveValidators)
			{
				IReadOnlyReactiveProperty<string>? errorMessage = validator(value);

				IDisposable? subscription = errorMessage?.Scan<string, (string Previous, string Current)>(
						("", ""),
						(accumulator, current) => (accumulator.Current, current))
					.Subscribe(
						x =>
						{
							_errors.Remove(x.Previous);
							_errors.Add(x.Current);

							_source.InvokeErrorsChanged();
						});

				if (subscription != null)
				{
					_reactiveValidatorSubscriptions[validator] = subscription;
				}
			}

			_source.InvokeErrorsChanged();
		}
	}
}