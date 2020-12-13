using System;
using System.Reactive.Linq;

namespace Blastic.Reactive
{
	public static class ReactivePropertyExtensions
	{
		/// <summary>
		/// Returns an observable that emits true if none of the given properties has data errors.
		/// </summary>
		/// <param name="properties">The reactive properties to check for data errors.</param>
		/// <returns>An observable that emits true if none of the given properties has data errors.</returns>
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