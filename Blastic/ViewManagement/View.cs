using System;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using Blastic.Initialization;

namespace Blastic.ViewManagement
{
	public static class View
	{
		private static readonly ContentPropertyAttribute DefaultContentProperty = new ContentPropertyAttribute("Content");

		public static readonly DependencyProperty ModelProperty = DependencyProperty.RegisterAttached(
			nameof(ModelProperty).Replace("Property", ""),
			typeof(object),
			typeof(View),
			new PropertyMetadata(default, OnModelChanged));
		public static object GetModel(DependencyObject obj) => obj.GetValue(ModelProperty);
		public static void SetModel(DependencyObject obj, object value) => obj.SetValue(ModelProperty, value);

		private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue == null)
			{
				SetContentProperty(d, e.NewValue);
				return;
			}

			UIElement view = BlasticApplication.ViewLocator.Locate(e.NewValue);

			if (!SetContentProperty(d, view))
			{
				view = BlasticApplication.ViewLocator.Locate(e.NewValue.GetType());
				SetContentProperty(d, view);
			}
		}

		private static bool SetContentProperty(object targetLocation, object view)
		{
			if (view is FrameworkElement f && f.Parent != null)
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