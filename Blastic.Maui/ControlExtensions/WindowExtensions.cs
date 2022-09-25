using System.ComponentModel;
using Bindables.Maui;
using Microsoft.Maui.Controls;

namespace Blastic.Maui.ControlExtensions;

public static partial class WindowExtensions
{
	[AttachedProperty(typeof(string), OnPropertyChanged = nameof(OnTitleChanged))]
	public static readonly BindableProperty TitleProperty;

	public static void OnTitleChanged(BindableObject target, object oldValue, object newValue)
	{
		if (target is not VisualElement element)
		{
			return;
		}

		if (element.Window == null)
		{
			element.PropertyChanged += OnPropertyChanged;
			return;
		}

		element.Window.Title = (string)newValue;
	}

	private static void OnPropertyChanged(object? sender, PropertyChangedEventArgs args)
	{
		if (sender is not VisualElement element)
		{
			return;
		}

		if (args.PropertyName == nameof(Window) && element.Window != null)
		{
			element.Window.Title = GetTitle(element);
		}
	}
}