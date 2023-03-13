using System;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Platform;
using Microsoft.Maui.ApplicationModel;

namespace Blastic.Maui.Platform;

public class MauiPlatformSpecifics : IPlatformSpecifics
{
	public IScheduler UIThreadScheduler { get; }

	public MauiPlatformSpecifics()
	{
		SynchronizationContext synchronizationContext = MainThread.GetMainThreadSynchronizationContextAsync().Result;
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
		if (MainThread.IsMainThread)
		{
			action();
			return;
		}

		MainThread.BeginInvokeOnMainThread(action);
	}

	public async Task OnUIThread(Func<Task> func)
	{
		await MainThread.InvokeOnMainThreadAsync(func);
	}
}