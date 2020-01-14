using System;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using System.Windows.Threading;
using Blastic.Platform;

namespace Blastic.Wpf.Platform
{
	public class WpfPlatformSpecifics : IPlatformSpecifics
	{
		private readonly Dispatcher _dispatcher;

		public WpfPlatformSpecifics(Dispatcher dispatcher)
		{
			_dispatcher = dispatcher;
		}

		public IObservable<T> ObserveOnUI<T>(IObservable<T> observable)
		{
			return Synchronization.ObserveOn(
				observable,
				new DispatcherSynchronizationContext(_dispatcher));
		}

		public void OnUIThread(Action action)
		{
			if (_dispatcher.CheckAccess())
			{
				action();
				return;
			}

			_dispatcher.Invoke(action);
		}

		public async Task OnUIThread(Func<Task> func)
		{
			await _dispatcher.InvokeAsync(func).Task;
		}
	}
}