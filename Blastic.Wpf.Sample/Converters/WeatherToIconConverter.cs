using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Blastic.Wpf.Sample.UserInterface;

namespace Blastic.Wpf.Sample.Converters
{
	public class WeatherToIconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value is Weather weather
				? $"pack://application:,,,/Resources/{weather}.svg"
				: DependencyProperty.UnsetValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}