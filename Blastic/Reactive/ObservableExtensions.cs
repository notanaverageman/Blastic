using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

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
		
		public static IObservable<bool> IsNotNull<T>(this IObservable<T> observable)
		{
			return observable.Select(x => x != null);
		}

		// https://stackoverflow.com/a/65155886/3670437
		public static IObservable<T> DisposePrevious<T>(this IObservable<T> source) where T : IDisposable
		{
			return Observable.Using(
				() => new SerialDisposable(),
				serial => source.Do(x => serial.Disposable = x));
		}

		// https://stackoverflow.com/a/50096940/3670437
		public static IDisposable SubscribeAsync<T>(
			this IObservable<T> source,
			Func<Task> action,
			Action<Exception>? handler = null)
		{
			async Task<Unit> Wrapped(T _)
			{
				await action();
				return Unit.Default;
			}

			return handler == null
				? source.SelectMany(Wrapped).Subscribe(_ => { })
				: source.SelectMany(Wrapped).Subscribe(_ => { }, handler);
		}

		public static IDisposable SubscribeAsync<T>(
			this IObservable<T> source,
			Func<T, Task> action,
			Action<Exception>? handler = null)
		{
			async Task<Unit> Wrapped(T t)
			{
				await action(t);
				return Unit.Default;
			}

			return handler == null
				? source.SelectMany(Wrapped).Subscribe(_ => { })
				: source.SelectMany(Wrapped).Subscribe(_ => { }, handler);
		}

		// https://stackoverflow.com/a/45217578/3670437
		public static IObservable<T> DoAsync<T>(
			this IObservable<T> source,
			Func<Task> action)
		{
			return source
				.Materialize()
				.SelectMany(async x =>
				{
					switch (x.Kind)
					{
						case NotificationKind.OnNext:
							await action();
							return x;

						case NotificationKind.OnCompleted:
						case NotificationKind.OnError:
							return x;

						default:
							throw new NotImplementedException();
					}
				})
				.Dematerialize();
		}
		
		public static IObservable<T> DoAsync<T>(
			this IObservable<T> source,
			Func<T, Task> action)
		{
			return source
				.Materialize()
				.SelectMany(async x =>
				{
					switch (x.Kind)
					{
						case NotificationKind.OnNext:
							await action(x.Value);
							return x;

						case NotificationKind.OnCompleted:
						case NotificationKind.OnError:
							return x;

						default:
							throw new NotImplementedException();
					}
				})
				.Dematerialize();
		}
	}
}