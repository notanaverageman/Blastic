using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Blastic.Wpf.Sample.Converters
{
	public class EnumToResourceConverter : IValueConverter
	{
		public string FileExtension { get; set; }

		public EnumToResourceConverter()
		{
			FileExtension = "";
		}
		
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!value.GetType().IsEnum)
			{
				return DependencyProperty.UnsetValue;
			}
			
			return $"pack://application:,,,/Resources/{value}.{FileExtension}";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}