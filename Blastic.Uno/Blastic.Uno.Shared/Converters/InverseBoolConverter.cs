using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Blastic.Converters
{
	public class InverseBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value is bool boolean)
			{
				return !boolean;
			}
			
			return DependencyProperty.UnsetValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			return Convert(value, targetType, parameter, language);
		}
	}
}