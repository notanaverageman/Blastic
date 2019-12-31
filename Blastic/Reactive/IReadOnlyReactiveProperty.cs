using System;
using System.ComponentModel;

namespace Blastic.Reactive
{
	public interface IReadOnlyReactiveProperty : INotifyPropertyChanged, INotifyDataErrorInfo
	{
		object Value { get; }
	}

	public interface IReadOnlyReactiveProperty<T> : IReadOnlyReactiveProperty, IObservable<T>
	{
		new T Value { get; }

		IDisposable Subscribe(IObserver<T> observer, bool raiseLatestValue);
		void AddValidator(Func<T, string> validator);
	}
}