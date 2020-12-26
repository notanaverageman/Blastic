using System;
using System.Reactive.Linq;

namespace Blastic.Reactive
{
	public static class ObservableExtensions
	{
		public static IObservable<bool> Not(this IObservable<bool> observable)
		{
			return observable.Select(x => !x);
		}
		
		public static IObservable<bool> And(this IObservable<bool> observable, IObservable<bool> other)
		{
			return observable.CombineLatest(other, (x, y) => x && y);
		}
		
		public static IObservable<bool> Or(this IObservable<bool> observable, IObservable<bool> other)
		{
			return observable.CombineLatest(other, (x, y) => x || y);
		}
	}
}