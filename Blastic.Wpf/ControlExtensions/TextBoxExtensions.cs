using System;
using System.Windows;
using System.Windows.Controls;

namespace Blastic.ControlExtensions
{
	public static class TextBoxExtensions
	{
		public static readonly DependencyProperty MoveCaretToEndWhenFocusedProperty = DependencyProperty.RegisterAttached(
			nameof(MoveCaretToEndWhenFocusedProperty).Replace("Property", ""),
			typeof(bool),
			typeof(TextBoxExtensions),
			new PropertyMetadata(default(bool), OnMoveCaretToEndWhenFocusedChanged));
		public static bool GetMoveCaretToEndWhenFocused(DependencyObject obj) => (bool)obj.GetValue(MoveCaretToEndWhenFocusedProperty);
		public static void SetMoveCaretToEndWhenFocused(DependencyObject obj, bool value) => obj.SetValue(MoveCaretToEndWhenFocusedProperty, value);

		private static void OnMoveCaretToEndWhenFocusedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!(d is TextBox textBox))
			{
				throw new ArgumentException("This property should be attached to a TextBox");
			}

			void GotFocus(object sender, RoutedEventArgs args)
			{
				TextBox t = (TextBox)sender;
				t.CaretIndex = int.MaxValue;
			}

			if ((bool)e.NewValue)
			{
				textBox.GotFocus += GotFocus;
			}
			else
			{
				textBox.GotFocus -= GotFocus;
			}
		}
	}
}