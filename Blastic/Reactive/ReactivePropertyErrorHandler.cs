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
		private readonly Dictionary<Func<T, bool>, IReadOnlyReactiveProperty<string>> _reactiveValidators;
		private readonly Dictionary<IReadOnlyReactiveProperty<string>, bool> _reactiveValidatorIsValid;

		public IEnumerable<string> Errors => _errors;

		public bool HasErrors => _errors.Count > 0;
		public IObservable<bool> HasErrorObservable { get; }

		public ReactivePropertyErrorHandler(ReactivePropertyBase<T> source)
		{
			_source = source;
			_errors = new List<string>();

			_validators = new List<Func<T, string?>>();

			_reactiveValidators = new Dictionary<Func<T, bool>, IReadOnlyReactiveProperty<string>>();
			_reactiveValidatorIsValid = new Dictionary<IReadOnlyReactiveProperty<string>, bool>();

			HasErrorObservable = Observable
				.FromEventPattern<DataErrorsChangedEventArgs>(
					x => source.ErrorsChanged += x,
					x => source.ErrorsChanged -= x)
				.Select(x => HasErrors);
		}

		public void AddValidator(Func<T, string?> validator)
		{
			_validators.Add(validator);
		}

		public void AddValidator(Func<T, bool> validator, IReadOnlyReactiveProperty<string> errorMessage)
		{
			_reactiveValidators.Add(validator, errorMessage);

			errorMessage
				.Scan<string, (string Previous, string Current)>(
					("", ""),
					(accumulator, current) => (accumulator.Current, current))
				.Subscribe(
					x =>
					{
						if (!_reactiveValidatorIsValid.TryGetValue(errorMessage, out bool isValid))
						{
							return;
						}

						if (isValid)
						{
							return;
						}

						_errors.Remove(x.Previous);
						_errors.Add(x.Current);

						_source.InvokeErrorsChanged();
					});
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

			foreach (KeyValuePair<Func<T, bool>, IReadOnlyReactiveProperty<string>> pair in _reactiveValidators)
			{
				Func<T, bool> validator = pair.Key;
				IReadOnlyReactiveProperty<string> errorMessage = pair.Value;

				bool isValid = validator(value);

				_reactiveValidatorIsValid[errorMessage] = isValid;

				if (isValid)
				{
					continue;
				}

				_errors.Add(errorMessage.Value);
			}

			_source.InvokeErrorsChanged();
		}
	}
}