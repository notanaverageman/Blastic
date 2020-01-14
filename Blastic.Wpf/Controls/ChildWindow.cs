using System.Windows;
using System.Windows.Controls;

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

		private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ChildWindow childWindow = (ChildWindow)d;

			if ((bool)e.NewValue)
			{
				childWindow.Focus();
			}
		}
	}
}