using System;
using System.Reactive.Concurrency;
using System.Threading.Tasks;

namespace Blastic.Platform
{
	/// <summary>
	/// Default implementation of <see cref="IPlatformSpecifics"/>.
	/// </summary>
	public class DefaultPlatformSpecifics : IPlatformSpecifics
	{
		/// <inheritdoc />
		public IScheduler UIThreadScheduler => Scheduler.Default;

		/// <inheritdoc />
		public IObservable<T> ObserveOnUI<T>(IObservable<T> observable)
		{
			return observable;
		}

		/// <inheritdoc />
		public void OnUIThread(Action action)
		{
			action();
		}
	}
}