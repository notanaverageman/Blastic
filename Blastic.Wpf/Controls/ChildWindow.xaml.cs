using System.Windows;
using System.Windows.Markup;

namespace Blastic.Wpf.Controls
{
	[ContentProperty(nameof(ChildContent))]
	public partial class ChildWindow
	{
		public static readonly DependencyProperty ChildContentProperty = DependencyProperty.Register(
			nameof(ChildContent).Replace("Property", ""),
			typeof(object),
			typeof(ChildWindow),
			new PropertyMetadata(default));
		public object ChildContent
		{
			get => GetValue(ChildContentProperty);
			set => SetValue(ChildContentProperty, value);
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

		public ChildWindow()
		{
			InitializeComponent();
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