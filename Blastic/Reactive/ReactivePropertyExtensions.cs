using System;
using System.Reactive;
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

		/// <see cref="IReadOnlyReactiveProperty{T}.Subscribe(IObserver{T},bool)"/>
		/// <param name="property">The reactive property.</param>
		/// <param name="onNext">Action to invoke when the property changes.</param>
		/// <param name="raiseLatestValue">Whether to emit the current value upon subscription.</param>
		/// <returns>A disposable that unsubscribes the observer upon disposal.</returns>
		public static IDisposable Subscribe<T>(
			this IReadOnlyReactiveProperty<T> property,
			Action<T> onNext,
			bool raiseLatestValue)
		{
			return property.Subscribe(Observer.Create(onNext), raiseLatestValue);
		}
	}
}