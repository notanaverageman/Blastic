using System.Collections.Generic;
using Xamarin.Forms;

namespace Blastic.Forms.ControlExtensions
{
	public class StyleExtensions
	{
		private static readonly ListStringTypeConverter Converter = new();

		public static readonly BindableProperty StylesProperty = BindableProperty.CreateAttached(
			nameof(StylesProperty).Replace("Property", ""),
			typeof(string),
			typeof(StyleExtensions),
			default(string),
			propertyChanged: OnStylesChanged);

		public static string GetStyles(BindableObject obj) => (string)obj.GetValue(StylesProperty);
		public static void SetStyles(BindableObject obj, string value) => obj.SetValue(StylesProperty, value);

		private static void OnStylesChanged(BindableObject bindable, object oldValue, object newValue)
		{
			if (bindable is not NavigableElement element)
			{
				return;
			}

			string value = (string) newValue;

			if (string.IsNullOrWhiteSpace(value))
			{
				value = null;
			}

			element.StyleClass = (IList<string>) Converter.ConvertFromInvariantString(value);
		}
	}
}