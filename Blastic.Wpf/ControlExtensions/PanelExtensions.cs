using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Blastic.Wpf.ControlExtensions
{
	public class PanelExtensions
	{
		public static readonly DependencyProperty PaddingProperty = DependencyProperty.RegisterAttached(
			nameof(PaddingProperty).Replace("Property", ""),
			typeof(Thickness),
			typeof(PanelExtensions),
			new PropertyMetadata(default(Thickness)));
		public static Thickness GetPadding(DependencyObject obj) => (Thickness)obj.GetValue(PaddingProperty);
		public static void SetPadding(DependencyObject obj, Thickness value) => obj.SetValue(PaddingProperty, value);

		public static readonly DependencyProperty EnablePaddingProperty = DependencyProperty.RegisterAttached(
			nameof(EnablePaddingProperty).Replace("Property", ""),
			typeof(bool),
			typeof(PanelExtensions),
			new PropertyMetadata(default(bool), OnEnablePaddingChanged));
		public static bool GetEnablePadding(DependencyObject obj) => (bool)obj.GetValue(EnablePaddingProperty);
		public static void SetEnablePadding(DependencyObject obj, bool value) => obj.SetValue(EnablePaddingProperty, value);
		
		private static void OnEnablePaddingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!(d is FrameworkElement element))
			{
				return;
			}

			if (e.NewValue as bool? == true)
			{
				element.Loaded += OnElementLoaded;
			}
			else
			{
				element.Loaded -= OnElementLoaded;
			}
		}

		private static void OnElementLoaded(object sender, RoutedEventArgs args)
		{
			FrameworkElement element = (FrameworkElement)sender;
			BindMarginsOfChildren(element);
		}

		private static void BindMarginsOfChildren(FrameworkElement element)
		{
			int childCount = VisualTreeHelper.GetChildrenCount(element);

			for (int i = 0; i < childCount; i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(element, i);
				DependencyProperty marginProperty = GetMarginProperty(child);

				//  If we have a margin property, bind it to the padding.
				if (marginProperty != null)
				{
					Binding binding = new Binding
					{
						Source = element,
						Path = new PropertyPath(PaddingProperty)
					};

					//  Bind the child's margin to the grid's padding.
					BindingOperations.SetBinding(child, marginProperty, binding);
				}
			}
		}

		private static DependencyProperty GetMarginProperty(DependencyObject dependencyObject)
		{
			foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(dependencyObject))
			{
				DependencyPropertyDescriptor descriptor = DependencyPropertyDescriptor.FromProperty(propertyDescriptor);

				if (descriptor?.Name == "Margin")
				{
					return descriptor.DependencyProperty;
				}
			}

			return null;
		}
	}
}