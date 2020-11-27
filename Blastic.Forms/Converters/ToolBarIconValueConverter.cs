using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Blastic.Forms.Converters
{
	// https://github.com/xamarin/Xamarin.Forms/issues/12700
	public class ToolBarIconValueConverter : IValueConverter, IMarkupExtension
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return new FontImageSource
			{
				FontFamily = (OnPlatform<string>)Application.Current.Resources["MaterialFontFamily"],
				Glyph = (string)value
			};
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public object ProvideValue(IServiceProvider serviceProvider)
		{
			return this;
		}
	}
}