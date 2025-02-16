using Blastic.Platform;
using DynamicData;
using System.Collections.ObjectModel;
using System;

namespace Blastic.Reactive;

public static class ObservableListExtensions
{
	public static ReadOnlyObservableCollection<TResult> Bind<T, TResult>(
		this IObservableList<T> source,
		IPlatformSpecifics platformSpecifics,
		Func<IObservable<IChangeSet<T>>, IObservable<IChangeSet<TResult>>> modifier)
		where T : notnull
		where TResult : notnull
	{
		IObservable<IChangeSet<T>> observeOnUI = source
			.Connect()
			.ObserveOnUI(platformSpecifics);

		IObservable<IChangeSet<TResult>> observable = modifier(observeOnUI);

		observable
			.Bind(out ReadOnlyObservableCollection<TResult> collection)
			.Subscribe();

		return collection;
	}
	
	public static ReadOnlyObservableCollection<T> Bind<T>(
		this IObservableList<T> source,
		IPlatformSpecifics platformSpecifics,
		Func<IObservable<IChangeSet<T>>, IObservable<IChangeSet<T>>>? modifier = null)
		where T : notnull
	{
		return source.Connect().Bind(platformSpecifics, modifier);
	}
	
	public static ReadOnlyObservableCollection<T> Bind<T>(
		this IObservable<IChangeSet<T>> source,
		IPlatformSpecifics platformSpecifics,
		Func<IObservable<IChangeSet<T>>, IObservable<IChangeSet<T>>>? modifier = null)
		where T : notnull
	{
		IObservable<IChangeSet<T>> observeOnUI = source.ObserveOnUI(platformSpecifics);

		if (modifier != null)
		{
			observeOnUI = modifier(observeOnUI);
		}

		observeOnUI
			.Bind(out ReadOnlyObservableCollection<T> collection)
			.Subscribe();

		return collection;
	}
}