using System;
using System.Threading.Tasks;

namespace Blastic.Platform
{
	public interface IPlatformSpecifics
	{
		IObservable<T> ObserveOnUI<T>(IObservable<T> observable);

		void OnUIThread(Action action);
		Task OnUIThread(Func<Task> func);
	}
}