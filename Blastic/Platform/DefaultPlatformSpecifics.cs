using System;
using System.Threading.Tasks;

namespace Blastic.Platform
{
	public class DefaultPlatformSpecifics : IPlatformSpecifics
	{
		public IObservable<T> ObserveOnUI<T>(IObservable<T> observable)
		{
			return observable;
		}

		public void OnUIThread(Action action)
		{
			action();
		}

		public async Task OnUIThread(Func<Task> func)
		{
			await func();
		}
	}
}