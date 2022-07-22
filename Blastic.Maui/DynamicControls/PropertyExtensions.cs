using System;
using Blastic.DynamicControls.Properties;
using Microsoft.Maui.Controls;

namespace Blastic.Maui.DynamicControls;

public static class PropertyExtensions
{
	public static Microsoft.Maui.GridUnitType ToMaui(this GridUnitType gridUnitType)
	{
		return gridUnitType switch
		{
			GridUnitType.Auto  => Microsoft.Maui.GridUnitType.Auto,
			GridUnitType.Pixel => Microsoft.Maui.GridUnitType.Absolute,
			GridUnitType.Star  => Microsoft.Maui.GridUnitType.Star,
			_ => throw new ArgumentOutOfRangeException(nameof(gridUnitType), gridUnitType, null)
		};
	}

	public static Microsoft.Maui.GridLength ToMaui(this GridLength gridLength)
	{
		return new Microsoft.Maui.GridLength(gridLength.Value, gridLength.UnitType.ToMaui());
	}

	public static LayoutOptions ToMaui(this HorizontalAlignment horizontalAlignment)
	{
		return horizontalAlignment switch
		{
			HorizontalAlignment.Left    => LayoutOptions.Start,
			HorizontalAlignment.Center  => LayoutOptions.Center,
			HorizontalAlignment.Right   => LayoutOptions.End,
			HorizontalAlignment.Stretch => LayoutOptions.Fill,
			_ => throw new ArgumentOutOfRangeException(nameof(horizontalAlignment), horizontalAlignment, null)
		};
	}

	public static Microsoft.Maui.Thickness ToMaui(this Thickness thickness)
	{
		return new Microsoft.Maui.Thickness(thickness.Left, thickness.Top, thickness.Right, thickness.Bottom);
	}
}