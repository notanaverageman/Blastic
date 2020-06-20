using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Blastic.Reactive
{
	internal class ReactivePropertyErrorHandler<T>
	{
		private readonly ReactivePropertyBase<T> _source;
		private readonly List<Func<T, string>> _validators;
		private readonly List<string> _errors;

		public IEnumerable<string> Errors => _errors;

		public bool HasErrors => _errors.Count > 0;
		public IObservable<bool> HasErrorObservable { get; }

		public ReactivePropertyErrorHandler(ReactivePropertyBase<T> source)
		{
			_source = source;
			_validators = new List<Func<T, string>>();

			_errors = new List<string>();

			HasErrorObservable = Observable
				.FromEventPattern<DataErrorsChangedEventArgs>(
					x => source.ErrorsChanged += x,
					x => source.ErrorsChanged -= x)
				.Select(x => HasErrors);
		}

		public void AddValidator(Func<T, string> validator)
		{
			_validators.Add(validator);
		}

		public void TriggerValidation(T value)
		{
			_errors.Clear();

			foreach (Func<T, string> validator in _validators)
			{
				string error = validator(value);

				if (string.IsNullOrEmpty(error))
				{
					continue;
				}

				_errors.Add(error);
			}

			_source.InvokeErrorsChanged();
		}
	}
}