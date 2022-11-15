using System.Windows;
using System.Windows.Input;
using Blastic.ViewManagement;

namespace Blastic.Wpf.ControlExtensions
{
	public static class FocusExtensions
	{
		public static readonly DependencyProperty IsFocusedProperty = DependencyProperty.RegisterAttached(
			nameof(IsFocusedProperty).Replace("Property", ""),
			typeof(bool),
			typeof(FocusExtensions),
			new PropertyMetadata(default(bool), OnIsFocusedChanged));
		public static bool GetIsFocused(DependencyObject obj) => (bool)obj.GetValue(IsFocusedProperty);
		public static void SetIsFocused(DependencyObject obj, bool value) => obj.SetValue(IsFocusedProperty, value);

		public static void OnIsFocusedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			FrameworkElement control = (FrameworkElement)d;

			void GotFocus(object sender, RoutedEventArgs args)
			{
				SetIsFocused((FrameworkElement)sender, true);
			}

			void LostFocus(object sender, RoutedEventArgs args)
			{
				SetIsFocused((FrameworkElement)sender, false);
			}

			void IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs args)
			{
				FrameworkElement element = (FrameworkElement)sender;

				if (element.IsVisible && GetIsFocused(element))
				{
					element.IsVisibleChanged -= IsVisibleChanged;
					element.Focus();
				}
			}

			if (e.OldValue == null)
			{
				control.GotFocus += GotFocus;
				control.LostFocus += LostFocus;
			}

			if (!control.IsVisible)
			{
				control.IsVisibleChanged += IsVisibleChanged;
			}

			if ((bool)e.NewValue)
			{
				control.Focus();
			}
		}

		public static void SetFocus(this IViewAware viewAware, object bindingSource)
		{
			if (viewAware.View.Value is not DependencyObject view)
			{
				return;
			}

			FrameworkElement? control = VisualTreeExtensions.FindChild(view, bindingSource);

			if (control == null)
			{
				return;
			}

			Keyboard.Focus(control);
		}
	}
}