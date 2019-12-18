using System;
using System.ComponentModel;

namespace Blastic.Reactive
{
	public interface IReadOnlyReactiveProperty : INotifyPropertyChanged
	{
		object Value { get; }
	}

	public interface IReadOnlyReactiveProperty<out T> : IReadOnlyReactiveProperty, IObservable<T>
	{
		new T Value { get; }

		IDisposable Subscribe(IObserver<T> observer, bool raiseLatestValue);
	}
}