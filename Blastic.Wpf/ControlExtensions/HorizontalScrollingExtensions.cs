using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Blastic.ControlExtensions
{
    public class HorizontalScrollingExtensions
    {
		public static readonly DependencyProperty EnableHorizontalScrollingWithShiftKeyProperty = DependencyProperty.RegisterAttached(
			nameof(EnableHorizontalScrollingWithShiftKeyProperty).Replace("Property", ""),
			typeof(bool),
			typeof(HorizontalScrollingExtensions),
			new PropertyMetadata(default(bool), OnEnableHorizontalScrollingWithShiftKeyChanged));
		public static bool GetEnableHorizontalScrollingWithShiftKey(DependencyObject obj) => (bool)obj.GetValue(EnableHorizontalScrollingWithShiftKeyProperty);
		public static void SetEnableHorizontalScrollingWithShiftKey(DependencyObject obj, bool value) => obj.SetValue(EnableHorizontalScrollingWithShiftKeyProperty, value);
		
        private static void OnEnableHorizontalScrollingWithShiftKeyChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
	        if (!(dp is UIElement uiElement))
            {
                return;
            }

            bool wasHandling = (bool)(e.OldValue);
            bool needToHandle = (bool)(e.NewValue);

            if (wasHandling)
            {
                uiElement.PreviewMouseWheel -= OnPreviewMouseWheel;
            }

            if (needToHandle)
            {
                uiElement.PreviewMouseWheel += OnPreviewMouseWheel;
            }
        }

        private static void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
	        UIElement uiElement = (UIElement)sender;
            ScrollViewer scrollViewer = GetScrollViewer(uiElement);

            if (Keyboard.Modifiers != ModifierKeys.Shift)
            {
                return;
            }

            scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - e.Delta);
            e.Handled = true;
        }

        private static ScrollViewer GetScrollViewer(UIElement element)
        {
	        if (element == null)
	        {
		        return null;
	        }

	        if (element is ScrollViewer s)
	        {
		        return s;
	        }

            ScrollViewer result = null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                if (VisualTreeHelper.GetChild(element, i) is ScrollViewer scrollViewer)
                {
                    result = scrollViewer;
                    break;
                }

                result = GetScrollViewer(VisualTreeHelper.GetChild(element, i) as UIElement);
            }

            return result;
        }
    }
}