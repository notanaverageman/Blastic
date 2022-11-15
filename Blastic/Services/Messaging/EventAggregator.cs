using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Blastic.Platform;

namespace Blastic.Services.Messaging
{
	/// <summary>
	/// Default implementation of <see cref="IEventAggregator"/>.
	/// </summary>
	public class EventAggregator : IEventAggregator
	{
		private readonly IPlatformSpecifics _platformSpecifics;
		private readonly Subject<object?> _subject;

		/// <summary>
		/// Creates a new instance of <see cref="EventAggregator"/>.
		/// </summary>
		/// <param name="platformSpecifics">Platform specifics to access the UI thread.</param>
		public EventAggregator(IPlatformSpecifics platformSpecifics)
		{
			_platformSpecifics = platformSpecifics;
			_subject = new Subject<object?>();
		}

		/// <inheritdoc />
		public IObservable<T> GetEventBus<T>()
		{
			return _subject!.OfType<T>().AsObservable();
		}

		/// <inheritdoc />
		public IDisposable Subscribe<T>(Action<T> action)
		{
			return GetEventBus<T>().Subscribe(action);
		}

		/// <inheritdoc />
		public IDisposable SubscribeOnUIThread<T>(Action<T> action)
		{
			return GetEventBus<T>()
				.ObserveOnUI(_platformSpecifics)
				.Subscribe(action);
		}

		/// <inheritdoc />
		public void Publish<T>(T @event)
		{
			_subject.OnNext(@event);
		}

		/// <inheritdoc />
		public void Dispose()
		{
			_subject.Dispose();
		}
	}
}