using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Blastic.Wpf.Converters
{
	public class NumberToStarGridLengthConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double d = GetValue(value);

			if (double.IsNaN(d))
			{
				d = 0;
			}

			return new GridLength(d, GridUnitType.Star);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private double GetValue(object value)
		{
			switch (value)
			{
				case int i:
					return i;
				case long l:
					return l;
				case float f:
					return f;
				case double d:
					return d;
			}

			throw new ArgumentException(nameof(value));
		}
	}
}