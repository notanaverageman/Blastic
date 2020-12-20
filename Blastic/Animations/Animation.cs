using System;
using System.Diagnostics;
using System.Reactive.Linq;

namespace Blastic.Animations
{
	/// <summary>
	/// Animation related extension methods.
	/// </summary>
	public static class Animation
	{
		/// <summary>
		/// Create an observable that will emit values between 0 and 1 in given duration.
		/// </summary>
		/// <param name="duration">Duration of the animation.</param>
		/// <param name="easing">Easing function to apply to values.</param>
		/// <param name="clock">
		/// An observable that emits <see cref="TimeSpan"/> values periodically.
		/// If not given, an observable that emits a value per 16ms will be used.
		/// </param>
		/// <returns>An observable that will emit values between 0 and 1 in given duration.</returns>
		public static IObservable<double> Create(
			TimeSpan duration,
			Func<double, double>? easing = null,
			IObservable<TimeSpan>? clock = null)
		{
			Stopwatch stopwatch = new();

			clock ??= Observable
				.Timer(TimeSpan.Zero, TimeSpan.FromMilliseconds(16))
				.Select(_ =>
				{
					if (stopwatch.IsRunning)
					{
						return stopwatch.Elapsed;
					}

					stopwatch.Start();
					return TimeSpan.Zero;
				});

			return clock
				.Select(x => x < duration ? x : duration)
				.TakeUntil(x => x.TotalMilliseconds >= duration.TotalMilliseconds)
				.Select(
					x =>
					{
						double progress = x.TotalMilliseconds / duration.TotalMilliseconds;

						if (easing != null)
						{
							progress = easing(progress);
						}

						return progress;
					});
		}
	}
}