using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Blastic.Converters
{
	public class InverseVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value is Visibility visibility)
			{
				return visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
			}
			
			return DependencyProperty.UnsetValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}