using System;
using System.Collections.ObjectModel;
using Blastic.Platform;
using DynamicData;

namespace Blastic.Reactive;

public class SourceListExtensions
{
	public static (SourceList<T> Source, ReadOnlyObservableCollection<T> Collection) CreateAndBind<T>(IPlatformSpecifics platformSpecifics)
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