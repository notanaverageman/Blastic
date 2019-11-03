using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Reactive.Bindings.Extensions;

namespace Blastic.Messaging
{
	public class EventAggregator : IEventAggregator
	{
		private readonly Subject<object> _subject = new Subject<object>();

		public IObservable<T> GetEventBus<T>()
		{
			return _subject.OfType<T>().AsObservable();
		}

		public IDisposable Subscribe<T>(Action<T> action)
		{
			return GetEventBus<T>().Subscribe(action);
		}

		public IDisposable SubscribeOnUIThread<T>(Action<T> action)
		{
			return GetEventBus<T>()
				.ObserveOnUIDispatcher()
				.Subscribe(action);
		}

		public void Publish<T>(T @event)
		{
			_subject.OnNext(@event);
		}

		public void Dispose()
		{
			_subject.Dispose();
		}
	}
}