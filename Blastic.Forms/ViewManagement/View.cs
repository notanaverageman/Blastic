using System;
using System.Linq;
using Blastic.Forms.Initialization;
using Xamarin.Forms;

namespace Blastic.Forms.ViewManagement
{
	public static class View
	{
		private static readonly ContentPropertyAttribute DefaultContentProperty = new ContentPropertyAttribute("Content");

		public static readonly BindableProperty ModelProperty = BindableProperty.CreateAttached(
			nameof(ModelProperty).Replace("Property", ""),
			typeof(object),
			typeof(View),
			default,
			propertyChanged: OnModelChanged);
		public static object GetModel(BindableObject obj) => obj.GetValue(ModelProperty);
		public static void SetModel(BindableObject obj, object value) => obj.SetValue(ModelProperty, value);

		private static void OnModelChanged(BindableObject bindable, object oldValue, object newValue)
		{
			if (newValue == null)
			{
				SetContentProperty(bindable, null);
				return;
			}

			VisualElement view = BlasticApplication.ViewLocator.Locate(newValue);

			if (!SetContentProperty(bindable, view))
			{
				view = BlasticApplication.ViewLocator.Locate(newValue.GetType());
				SetContentProperty(bindable, view);
			}
		}

		private static bool SetContentProperty(object targetLocation, object view)
		{
			if (view is VisualElement f && f.Parent != null)
			{
				SetContentPropertyCore(f.Parent, null);
			}

			return SetContentPropertyCore(targetLocation, view);
		}

		private static bool SetContentPropertyCore(object targetLocation, object view)
		{
			try
			{
				Type type = targetLocation.GetType();

				ContentPropertyAttribute contentProperty = type
					.GetCustomAttributes(typeof(ContentPropertyAttribute), true)
					.OfType<ContentPropertyAttribute>()
					.FirstOrDefault();

				contentProperty ??= DefaultContentProperty;

				type.GetProperty(contentProperty.Name)?.SetValue(targetLocation, view, null);

				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}