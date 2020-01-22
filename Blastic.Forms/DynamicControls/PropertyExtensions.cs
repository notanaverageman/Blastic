using System;
using Blastic.DynamicControls.Properties;
using Xamarin.Forms;
using GridLength = Blastic.DynamicControls.Properties.GridLength;
using GridUnitType = Blastic.DynamicControls.Properties.GridUnitType;
using Thickness = Blastic.DynamicControls.Properties.Thickness;

namespace Blastic.Forms.DynamicControls
{
	public static class PropertyExtensions
	{
		public static Xamarin.Forms.GridUnitType ToXamarin(this GridUnitType gridUnitType)
		{
			switch (gridUnitType)
			{
				case GridUnitType.Auto:
					return Xamarin.Forms.GridUnitType.Auto;
				case GridUnitType.Pixel:
					return Xamarin.Forms.GridUnitType.Absolute;
				case GridUnitType.Star:
					return Xamarin.Forms.GridUnitType.Star;
				default:
					throw new ArgumentOutOfRangeException(nameof(gridUnitType), gridUnitType, null);
			}
		}

		public static Xamarin.Forms.GridLength ToXamarin(this GridLength gridLength)
		{
			return new Xamarin.Forms.GridLength(gridLength.Value, gridLength.UnitType.ToXamarin());
		}

		public static LayoutOptions ToXamarin(this HorizontalAlignment horizontalAlignment)
		{
			switch (horizontalAlignment)
			{
				case HorizontalAlignment.Left:
					return LayoutOptions.Start;
				case HorizontalAlignment.Center:
					return LayoutOptions.Center;
				case HorizontalAlignment.Right:
					return LayoutOptions.End;
				case HorizontalAlignment.Stretch:
					return LayoutOptions.FillAndExpand;
				default:
					throw new ArgumentOutOfRangeException(nameof(horizontalAlignment), horizontalAlignment, null);
			}
		}

		public static Xamarin.Forms.Thickness ToXamarin(this Thickness thickness)
		{
			return new Xamarin.Forms.Thickness(thickness.Left, thickness.Top, thickness.Right, thickness.Bottom);
		}
	}
}