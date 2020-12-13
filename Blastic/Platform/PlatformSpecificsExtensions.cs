using System;

namespace Blastic.Platform
{
	public static class PlatformSpecificsExtensions
	{
		/// <summary>
		/// Observe the given observable on current platform's UI thread.
		/// </summary>
		/// <typeparam name="T">Type of the observable.</typeparam>
		/// <param name="observable">Observable to observe.</param>
		/// <returns>The new observable that emits on UI thread.</returns>
		public static IObservable<T> ObserveOnUI<T>(this IObservable<T> observable)
		{
			return PlatformSpecifics.Current.ObserveOnUI(observable);
		}
	}
}