using System;

namespace Blastic.Platform
{
	public static class PlatformSpecificsExtensions
	{
		public static IObservable<T> ObserveOnUI<T>(this IObservable<T> observable)
		{
			return PlatformSpecifics.Current.ObserveOnUI(observable);
		}
	}
}