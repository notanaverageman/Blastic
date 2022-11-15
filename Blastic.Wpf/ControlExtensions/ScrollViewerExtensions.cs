using System.Windows;
using System.Windows.Controls;

namespace Blastic.Wpf.ControlExtensions
{
	public static class ScrollViewerExtensions
	{
		public static readonly DependencyProperty AutoScrollProperty = DependencyProperty.RegisterAttached(
			nameof(AutoScrollProperty).Replace("Property", ""),
			typeof(bool),
			typeof(ScrollViewerExtensions),
			new PropertyMetadata(default(bool), AutoScrollPropertyChanged));
		public static bool GetAutoScroll(DependencyObject obj) => (bool)obj.GetValue(AutoScrollProperty);
		public static void SetAutoScroll(DependencyObject obj, bool value) => obj.SetValue(AutoScrollProperty, value);
		
		public static void AutoScrollPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
		{
			if (!(d is ScrollViewer scrollViewer))
			{
				return;
			}

			if ((bool)args.NewValue)
			{
				scrollViewer.ScrollChanged += OnScrollChanged;
				scrollViewer.ScrollToEnd();
			}
			else
			{
				scrollViewer.ScrollChanged -= OnScrollChanged;
			}
		}

		private static void OnScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			// Only scroll to bottom when the extent changed. Otherwise you can't scroll up.
			// ReSharper disable once CompareOfFloatsByEqualityOperator
			if (e.ExtentHeightChange == 0)
			{
				return;
			}

			ScrollViewer? scrollViewer = sender as ScrollViewer;
			scrollViewer?.ScrollToBottom();
		}
	}
}