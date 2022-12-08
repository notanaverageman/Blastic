using System.Reactive.Concurrency;
using Avalonia.Threading;
using Blastic.Platform;

namespace Blastic.Avalonia.Platform;

public class AvaloniaPlatformSpecifics : IPlatformSpecifics
{
	private readonly IDispatcher _dispatcher;

	public IScheduler UIThreadScheduler { get; }

	public AvaloniaPlatformSpecifics()
	{
		_dispatcher = Dispatcher.UIThread;
		UIThreadScheduler = AvaloniaScheduler.Instance;
	}

	public IObservable<T> ObserveOnUI<T>(IObservable<T> observable)
	{
		return Synchronization.ObserveOn(
			observable,
			UIThreadScheduler);
	}

	public void OnUIThread(Action action)
	{
		if (_dispatcher.CheckAccess())
		{
			action();
			return;
		}

		_dispatcher.Post(action);
	}

	public async Task OnUIThread(Func<Task> func)
	{
		await _dispatcher.InvokeAsync(func);
	}
}