using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Blastic.Wpf.Controls
{
	public class ChildWindow : ContentControl
	{
		static ChildWindow()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ChildWindow), new FrameworkPropertyMetadata(typeof(ChildWindow)));
		}

		public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
			nameof(IsOpen).Replace("Property", ""),
			typeof(bool),
			typeof(ChildWindow),
			new PropertyMetadata(default(bool), OnIsOpenChanged));
		public bool IsOpen
		{
			get => (bool)GetValue(IsOpenProperty);
			set => SetValue(IsOpenProperty, value);
		}

		public static readonly DependencyProperty OverlayBrushProperty = DependencyProperty.Register(
			nameof(OverlayBrush).Replace("Property", ""),
			typeof(Brush),
			typeof(ChildWindow),
			new PropertyMetadata(Brushes.Transparent));
		public Brush OverlayBrush
		{
			get => (Brush)GetValue(OverlayBrushProperty);
			set => SetValue(OverlayBrushProperty, value);
		}

		private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ChildWindow childWindow = (ChildWindow)d;

			if ((bool)e.NewValue)
			{
				childWindow.Focus();
			}

			VisualStateManager.GoToState(childWindow, (bool)e.NewValue == false ? "Hide" : "Show", true);
		}
	}
}