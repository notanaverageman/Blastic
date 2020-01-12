using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Blastic.ControlExtensions
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
				if(scrollViewer.Content is FrameworkElement child)
				{
					child.SizeChanged += OnSizeChanged;
				}

				scrollViewer.ChangeView(null, scrollViewer.ScrollableHeight, null);
			}
			else
			{
				if (scrollViewer.Content is FrameworkElement child)
				{
					child.SizeChanged -= OnSizeChanged;
				}

				scrollViewer.ChangeView(null, scrollViewer.ScrollableHeight, null);
			}

			void OnSizeChanged(object sender, SizeChangedEventArgs e)
			{
				scrollViewer.ChangeView(null, scrollViewer.ScrollableHeight, null);
			}
		}
	}
}