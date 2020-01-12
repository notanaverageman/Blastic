using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Blastic.Converters
{
	public class BoolToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value is bool boolean)
			{
				return boolean ? Visibility.Visible : Visibility.Collapsed;
			}
			
			return DependencyProperty.UnsetValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}