using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Blastic.Converters
{
	public class IntToByteSizeConverter : IValueConverter
	{
		private const long OneKb = 1024;
		private const long OneMb = OneKb * 1024;
		private const long OneGb = OneMb * 1024;
		private const long OneTb = OneGb * 1024;

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value is long l)
			{
				return ToPrettySize(l);
			}

			if (value is int i)
			{
				return ToPrettySize(i);
			}

			return DependencyProperty.UnsetValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}

		public static string ToPrettySize(long value, int decimalPlaces = 2)
		{
			double asTb = Math.Round((double)value / OneTb, decimalPlaces);
			double asGb = Math.Round((double)value / OneGb, decimalPlaces);
			double asMb = Math.Round((double)value / OneMb, decimalPlaces);
			double asKb = Math.Round((double)value / OneKb, decimalPlaces);

			string chosenValue
				= asTb > 1 ? $"{asTb}TB"
				: asGb > 1 ? $"{asGb}GB"
				: asMb > 1 ? $"{asMb}MB"
				: asKb > 1 ? $"{asKb}KB"
				: $"{Math.Round((double) value, decimalPlaces)}B";

			return chosenValue;
		}
	}
}