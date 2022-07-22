using System;
using System.Linq;
using System.Reflection;
using Microsoft.Maui.Controls;

namespace Blastic.Maui.ViewManagement;

public static class View
{
	private static readonly ContentPropertyAttribute DefaultContentProperty = new("Content");

	public static readonly BindableProperty ModelProperty = BindableProperty.CreateAttached(
		nameof(ModelProperty).Replace("Property", ""),
		typeof(object),
		typeof(View),
		default,
		propertyChanged: OnModelChanged);
	public static object GetModel(BindableObject obj) => obj.GetValue(ModelProperty);
	public static void SetModel(BindableObject obj, object value) => obj.SetValue(ModelProperty, value);

	public static readonly BindableProperty ItemTemplateProperty = BindableProperty.CreateAttached(
		nameof(ItemTemplateProperty).Replace("Property", ""),
		typeof(object),
		typeof(View),
		default,
		propertyChanged: OnItemTemplateChanged);
	public static object GetItemTemplate(BindableObject obj) => obj.GetValue(ItemTemplateProperty);
	public static void SetItemTemplate(BindableObject obj, object value) => obj.SetValue(ItemTemplateProperty, value);

	private static void OnModelChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (newValue == null)
		{
			SetContentProperty(bindable, null);
			return;
		}

		VisualElement view = ViewLocator.Current.Locate(newValue);

		if (!SetContentProperty(bindable, view))
		{
			view = ViewLocator.Current.Locate(newValue.GetType());
			SetContentProperty(bindable, view);
		}
	}

	private static void OnItemTemplateChanged(BindableObject bindable, object oldValue, object newValue)
	{
		Type type = bindable.GetType();

		DataTemplateSelector template = null;

		if (newValue != null)
		{
			template = new ViewDataTemplateSelector();
		}

		bindable.SetValue(MultiPage<Page>.ItemTemplateProperty, template);
	}

	private static bool SetContentProperty(object targetLocation, object view)
	{
		if (view is VisualElement { Parent: { } } f)
		{
			SetContentPropertyCore(f.Parent, null);
		}

		return SetContentPropertyCore(targetLocation, view);
	}

	private static bool SetContentPropertyCore(object targetLocation, object view)
	{
		Type type = targetLocation.GetType();

		ContentPropertyAttribute contentProperty = type
			.GetCustomAttributes(typeof(ContentPropertyAttribute), true)
			.OfType<ContentPropertyAttribute>()
			.FirstOrDefault();

		contentProperty ??= DefaultContentProperty;

		PropertyInfo propertyInfo = type.GetProperty(contentProperty.Name);

		if (propertyInfo == null)
		{
			return false;
		}

		propertyInfo.SetValue(targetLocation, view, null);

		return true;
	}

	private class ViewDataTemplateSelector : DataTemplateSelector
	{
		protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
		{
			return new DataTemplate(() => ViewLocator.Current.Locate(item));
		}
	}
}