using Bindables.Maui;
using Microsoft.Maui.Controls;

namespace Blastic.Maui.ControlExtensions;

public static partial class MouseExtensions
{
	[AttachedProperty(typeof(bool))]
	public static readonly BindableProperty IsPointerOverProperty;

	[AttachedProperty(typeof(bool), OnPropertyChanged = nameof(UsePointerEnteredExitedChanged))]
	public static readonly BindableProperty UsePointerEnteredExitedProperty;

	private static void UsePointerEnteredExitedChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is not VisualElement visual)
		{
			return;
		}

		bool useMouseOver = (bool)newValue;

		UpdatePointerEnteredExitedSubscription(visual, useMouseOver);
	}

	private static partial void UpdatePointerEnteredExitedSubscription(VisualElement visual, bool useMouseOver);
}