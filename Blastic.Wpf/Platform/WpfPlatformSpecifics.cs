using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using Blastic.Platform;

namespace Blastic.Wpf.Platform
{
	public class WpfPlatformSpecifics : IPlatformSpecifics
	{
		private readonly SynchronizationContext _synchronizationContext;

		public IScheduler UIThreadScheduler { get; }

		public WpfPlatformSpecifics(SynchronizationContext synchronizationContext)
		{
			_synchronizationContext = synchronizationContext;

			UIThreadScheduler = new SynchronizationContextScheduler(synchronizationContext);
		}

		public IObservable<T> ObserveOnUI<T>(IObservable<T> observable)
		{
			return observable.ObserveOn(UIThreadScheduler);
		}

		public void OnUIThread(Action action)
		{
			_synchronizationContext.Send(_ => { action(); }, null);
		}
	}
}