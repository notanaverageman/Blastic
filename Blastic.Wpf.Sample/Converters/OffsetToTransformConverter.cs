using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Blastic.Wpf.Sample.Converters
{
	public class OffsetToTransformConverter : DependencyObject, IValueConverter
	{
		public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register(
			nameof(ScaleProperty).Replace("Property", ""),
			typeof(double),
			typeof(OffsetToTransformConverter),
			new PropertyMetadata(1.0));
		
		public double Scale
		{
			get => (double)GetValue(ScaleProperty);
			set => SetValue(ScaleProperty, value);
		}

		public bool ApplyX { get; set; }
		public bool ApplyY { get; set; }
		
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is not double offset)
			{
				return DependencyProperty.UnsetValue;
			}
			
			double xOffset = ApplyX ? offset * Scale : 0;
			double yOffset = ApplyY ? offset * Scale : 0;

			return new TranslateTransform(xOffset, yOffset);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}