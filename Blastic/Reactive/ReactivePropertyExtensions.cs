using System;
using System.Reactive.Linq;

namespace Blastic.Reactive
{
	public static class ReactivePropertyExtensions
	{
		public static IObservable<bool> NoErrors(params IReadOnlyReactiveProperty[] properties)
		{
			IObservable<bool> result = Observable.Repeat(true, 1);

			foreach (IReadOnlyReactiveProperty property in properties)
			{
				IObservable<bool>? hasErrorObservable = property.HasErrorObservable;

				if (hasErrorObservable == null)
				{
					continue;
				}

				result = result.CombineLatest(hasErrorObservable, (x, y) => !(x || y));
			}

			return result;
		}
	}
}