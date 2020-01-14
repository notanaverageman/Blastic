using System;

namespace Blastic.Services.Messaging
{
	public interface IEventAggregator : IDisposable
	{
		IObservable<T> GetEventBus<T>();

		IDisposable Subscribe<T>(Action<T> action);
		IDisposable SubscribeOnUIThread<T>(Action<T> action);

		void Publish<T>(T @event);
	}
}