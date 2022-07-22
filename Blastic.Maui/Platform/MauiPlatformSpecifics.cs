using System;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Platform;
using Microsoft.Maui.Dispatching;

namespace Blastic.Maui.Platform;

public class MauiPlatformSpecifics : IPlatformSpecifics
{
	private readonly IDispatcher _dispatcher;

	public IScheduler UIThreadScheduler { get; }

	public MauiPlatformSpecifics(IDispatcher dispatcher, SynchronizationContext synchronizationContext)
	{
		_dispatcher = dispatcher;
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
		if (!_dispatcher.IsDispatchRequired)
		{
			action();
			return;
		}

		_dispatcher.Dispatch(action);
	}

	public async Task OnUIThread(Func<Task> func)
	{
		await _dispatcher.DispatchAsync(func);
	}
}