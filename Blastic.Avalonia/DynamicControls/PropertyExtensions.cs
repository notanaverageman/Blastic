using Blastic.DynamicControls.Properties;

namespace Blastic.Avalonia.DynamicControls;

public static class PropertyExtensions
{
	public static global::Avalonia.Controls.GridUnitType ToAvalonia(this GridUnitType gridUnitType)
	{
		return gridUnitType switch
		{
			GridUnitType.Auto  => global::Avalonia.Controls.GridUnitType.Auto,
			GridUnitType.Pixel => global::Avalonia.Controls.GridUnitType.Pixel,
			GridUnitType.Star  => global::Avalonia.Controls.GridUnitType.Star,
			_ => throw new ArgumentOutOfRangeException(nameof(gridUnitType), gridUnitType, null)
		};
	}

	public static global::Avalonia.Controls.GridLength ToAvalonia(this GridLength gridLength)
	{
		return new global::Avalonia.Controls.GridLength(gridLength.Value, gridLength.UnitType.ToAvalonia());
	}

	public static global::Avalonia.Layout.HorizontalAlignment ToAvalonia(this HorizontalAlignment horizontalAlignment)
	{
		return horizontalAlignment switch
		{
			HorizontalAlignment.Left    => global::Avalonia.Layout.HorizontalAlignment.Left,
			HorizontalAlignment.Center  => global::Avalonia.Layout.HorizontalAlignment.Center,
			HorizontalAlignment.Right   => global::Avalonia.Layout.HorizontalAlignment.Right,
			HorizontalAlignment.Stretch => global::Avalonia.Layout.HorizontalAlignment.Stretch,
			_ => throw new ArgumentOutOfRangeException(nameof(horizontalAlignment), horizontalAlignment, null)
		};
	}

	public static global::Avalonia.Thickness ToMaui(this Thickness thickness)
	{
		return new global::Avalonia.Thickness(thickness.Left, thickness.Top, thickness.Right, thickness.Bottom);
	}
}