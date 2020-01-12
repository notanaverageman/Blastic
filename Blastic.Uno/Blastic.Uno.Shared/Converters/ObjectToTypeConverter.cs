using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Blastic.Converters
{
	public class ObjectToTypeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value == null)
			{
				return DependencyProperty.UnsetValue;
			}

			return value.GetType();
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}