using System;
using System.ComponentModel;

namespace Blastic.Reactive
{
	public interface IReadOnlyReactiveProperty : INotifyPropertyChanged, INotifyDataErrorInfo
	{
		object? Value { get; }

		void TriggerValidation();
		IObservable<bool>? HasErrorObservable { get; }
	}

	public interface IReadOnlyReactiveProperty<out T> : IReadOnlyReactiveProperty, IObservable<T>
	{
		new T Value { get; }

		IDisposable Subscribe(IObserver<T> observer, bool raiseLatestValue);

		void AddValidator(Func<T, string?> validator);
		void AddValidator(Func<T, bool> validator, IReadOnlyReactiveProperty<string> errorMessage);
	}
}