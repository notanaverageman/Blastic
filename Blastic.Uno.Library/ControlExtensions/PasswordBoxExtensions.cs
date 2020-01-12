using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Blastic.ControlExtensions
{
	public class PasswordBoxExtensions
	{
		public static readonly DependencyProperty BoundPasswordProperty = DependencyProperty.RegisterAttached(
			nameof(BoundPasswordProperty).Replace("Property", ""),
			typeof(string),
			typeof(PasswordBoxExtensions),
			new PropertyMetadata(default(string), OnPasswordChanged));
		public static string GetBoundPassword(DependencyObject obj) => (string)obj.GetValue(BoundPasswordProperty);
		public static void SetBoundPassword(DependencyObject obj, string value) => obj.SetValue(BoundPasswordProperty, value);

		public static readonly DependencyProperty BindPasswordProperty = DependencyProperty.RegisterAttached(
			nameof(BindPasswordProperty).Replace("Property", ""),
			typeof(bool),
			typeof(PasswordBoxExtensions),
			new PropertyMetadata(default(bool), OnBindPasswordChanged));
		public static bool GetBindPassword(DependencyObject obj) => (bool)obj.GetValue(BindPasswordProperty);
		public static void SetBindPassword(DependencyObject obj, bool value) => obj.SetValue(BindPasswordProperty, value);

		private static void OnPasswordChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			if (!(dependencyObject is PasswordBox passwordBox))
			{
				return;
			}

			string newValue = (string)e.NewValue;

			if (passwordBox.Password == newValue)
			{
				return;
			}

			passwordBox.Password = newValue;
		}

		private static void OnBindPasswordChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			if (!(dependencyObject is PasswordBox passwordBox))
			{
				return;
			}

			bool wasBound = (bool)e.OldValue;
			bool needToBind = (bool)e.NewValue;

			if (wasBound)
			{
				passwordBox.PasswordChanged -= HandlePasswordChanged;
			}

			if (needToBind)
			{
				passwordBox.PasswordChanged += HandlePasswordChanged;
			}
		}

		private static void HandlePasswordChanged(object sender, RoutedEventArgs e)
		{
			PasswordBox passwordBox = (PasswordBox)sender;
			SetBoundPassword(passwordBox, passwordBox.Password);
		}
	}
}