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
		private readonly SynchronizationContext _synchronizationContext;

		public FormsPlatformSpecifics(SynchronizationContext synchronizationContext)
		{
			_synchronizationContext = synchronizationContext;
		}

		public IObservable<T> ObserveOnUI<T>(IObservable<T> observable)
		{
			return Synchronization.ObserveOn(
				observable,
				_synchronizationContext);
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