using System;
using Blastic.DynamicControls.Properties;

namespace Blastic.Wpf.DynamicControls
{
	public static class PropertyExtensions
	{
		public static System.Windows.GridUnitType ToWpf(this GridUnitType gridUnitType)
		{
			switch (gridUnitType)
			{
				case GridUnitType.Auto:
					return System.Windows.GridUnitType.Auto;
				case GridUnitType.Pixel:
					return System.Windows.GridUnitType.Pixel;
				case GridUnitType.Star:
					return System.Windows.GridUnitType.Star;
				default:
					throw new ArgumentOutOfRangeException(nameof(gridUnitType), gridUnitType, null);
			}
		}

		public static System.Windows.GridLength ToWpf(this GridLength gridLength)
		{
			return new System.Windows.GridLength(gridLength.Value, gridLength.UnitType.ToWpf());
		}

		public static System.Windows.HorizontalAlignment ToWpf(this HorizontalAlignment horizontalAlignment)
		{
			switch (horizontalAlignment)
			{
				case HorizontalAlignment.Left:
					return System.Windows.HorizontalAlignment.Left;
				case HorizontalAlignment.Center:
					return System.Windows.HorizontalAlignment.Center;
				case HorizontalAlignment.Right:
					return System.Windows.HorizontalAlignment.Right;
				case HorizontalAlignment.Stretch:
					return System.Windows.HorizontalAlignment.Stretch;
				default:
					throw new ArgumentOutOfRangeException(nameof(horizontalAlignment), horizontalAlignment, null);
			}
		}

		public static System.Windows.Thickness ToWpf(this Thickness thickness)
		{
			return new System.Windows.Thickness(thickness.Left, thickness.Top, thickness.Right, thickness.Bottom);
		}
	}
}