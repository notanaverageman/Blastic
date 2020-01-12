using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Blastic.Converters
{
	public class NumberToStarGridLengthConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			double d = GetValue(value);
			return new GridLength(d, GridUnitType.Star);
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
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