using System;
using Windows.UI.Xaml.Data;

namespace Blastic.Converters
{
	public class TypeOfConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			return value?.GetType();
		}
		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}