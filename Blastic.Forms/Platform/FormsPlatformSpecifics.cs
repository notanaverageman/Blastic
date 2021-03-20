using System;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Platform;
using Xamarin.Forms;

namespace Blastic.Forms.Platform
{
	public class FormsPlatformSpecifics : IPlatformSpecifics
	{
		public IScheduler UIThreadScheduler { get; }

		public FormsPlatformSpecifics(SynchronizationContext synchronizationContext)
		{
			UIThreadScheduler = new SynchronizationContextScheduler(synchronizationContext);
		}

		public IObservable<T> ObserveOnUI<T>(IObservable<T> observable)
		{
			return Synchronization.ObserveOn(
				observable,
				UIThreadScheduler);
		}

		public void OnUIThread(Action action)
		{
			if (!Device.IsInvokeRequired)
			{
				action();
				return;
			}

			Device.BeginInvokeOnMainThread(action);
		}

		public async Task OnUIThread(Func<Task> func)
		{
			await Device.InvokeOnMainThreadAsync(func);
		}
	}
}