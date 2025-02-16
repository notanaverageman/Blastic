using System;
using System.Collections.ObjectModel;
using Blastic.Platform;
using DynamicData;

namespace Blastic.Reactive;

public static class SourceListExtensions
{
	public static ReadOnlyObservableCollection<T> Bind<T>(
		this SourceList<T> source,
		IPlatformSpecifics platformSpecifics,
		Func<IObservable<IChangeSet<T>>, IObservable<IChangeSet<T>>>? modifier = null)
		where T : notnull
	{
		return source.AsObservableList().Bind<T>(platformSpecifics, modifier);
	}

	public static ReadOnlyObservableCollection<T> Bind<T, TKey>(
		this SourceCache<T, TKey> source,
		IPlatformSpecifics platformSpecifics,
		Func<IObservable<IChangeSet<T, TKey>>, IObservable<IChangeSet<T, TKey>>>? modifier = null)
		where T : notnull
		where TKey : notnull
	{
		IObservable<IChangeSet<T, TKey>> observeOnUI = source
			.Connect()
			.ObserveOnUI(platformSpecifics);

		if (modifier != null)
		{
			observeOnUI = modifier(observeOnUI);
		}

		observeOnUI
			.Bind(out ReadOnlyObservableCollection<T> collection)
			.Subscribe();

		return collection;
	}

	public static (SourceList<T> Source, ReadOnlyObservableCollection<T> Collection) CreateAndBind<T>(
		IPlatformSpecifics platformSpecifics)
		where T : notnull
	{
		SourceList<T> source = new();

		source
			.Connect()
			.ObserveOnUI(platformSpecifics)
			.Bind(out ReadOnlyObservableCollection<T> collection)
			.Subscribe();

		return (source, collection);
	}
}