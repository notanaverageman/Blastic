using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Blastic.Wpf.Converters
{
	public class MultiValueConverterGroup : List<object>, IMultiValueConverter
	{
		public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			object? result = DependencyProperty.UnsetValue;

			object? converterInput = values;
			object[] multiConverterInput = values;

			for (int i = 0; i < Count; i++)
			{
				object converter = this[i];
				object? nextConverter = i < Count - 1 ? this[i + 1] : null;

				result = converter switch
				{
					IMultiValueConverter multiValueConverter => multiValueConverter.Convert(multiConverterInput, targetType, parameter, culture),
					IValueConverter valueConverter => valueConverter.Convert(converterInput, targetType, parameter, culture),
					_ => throw new ArgumentException(nameof(converter))
				};

				switch (nextConverter)
				{
					case IMultiValueConverter when result is object[] array:
						multiConverterInput = array;
						break;
					case IMultiValueConverter:
						multiConverterInput = new[] { result };
						break;
					case IValueConverter:
						converterInput = result;
						break;
				}
			}

			return result;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}