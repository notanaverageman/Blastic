using System;
using System.Threading.Tasks;

namespace Blastic.Platform
{
	/// <summary>
	/// An interface for running platform specific code in cross platform manner.
	/// </summary>
	public interface IPlatformSpecifics
	{
		/// <summary>
		/// Observe the given observable on platform's UI thread.
		/// </summary>
		/// <typeparam name="T">Type of the observable.</typeparam>
		/// <param name="observable">Observable to observe.</param>
		/// <returns>The new observable that emits on UI thread.</returns>
		IObservable<T> ObserveOnUI<T>(IObservable<T> observable);

		/// <summary>
		/// Run the given action on platform's UI thread.
		/// </summary>
		/// <param name="action">The action to run on UI thread.</param>
		void OnUIThread(Action action);

		/// <summary>
		/// Run the given action on platform's UI thread.
		/// </summary>
		/// <param name="func">The action to run on UI thread.</param>
		Task OnUIThread(Func<Task> func);
	}
}