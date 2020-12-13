using System;
using System.Threading.Tasks;

namespace Blastic.Platform
{
	/// <summary>
	/// Default implementation of <see cref="IPlatformSpecifics"/>.
	/// </summary>
	public class DefaultPlatformSpecifics : IPlatformSpecifics
	{
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

		/// <inheritdoc />
		public async Task OnUIThread(Func<Task> func)
		{
			await func();
		}
	}
}