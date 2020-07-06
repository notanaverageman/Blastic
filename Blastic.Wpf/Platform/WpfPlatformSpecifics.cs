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

			try
			{
				_dispatcher.Invoke(action);
			}
			catch (OperationCanceledException)
			{
				// This exception is thrown when application is shutdown while we
				// are waiting for the completion of the action. Ignore it.
			}
		}

		public async Task OnUIThread(Func<Task> func)
		{
			await _dispatcher.InvokeAsync(func).Task;
		}
	}
}