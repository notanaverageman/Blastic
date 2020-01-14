using System.Windows;
using System.Windows.Input;
using Blastic.Services.Notifications;

namespace Blastic.Wpf.Controls.Notifications
{
	public partial class NotificationView
	{
		public static readonly DependencyProperty NotificationProperty = DependencyProperty.Register(
			nameof(NotificationProperty).Replace("Property", ""),
			typeof(Notification),
			typeof(NotificationView),
			new PropertyMetadata(default(Notification)));
		public Notification Notification
		{
			get => (Notification)GetValue(NotificationProperty);
			set => SetValue(NotificationProperty, value);
		}

		private bool _hasMouseFocus;
		private bool _hasKeyboardFocus;

		public NotificationView()
		{
			InitializeComponent();
		}

		protected override void OnMouseEnter(MouseEventArgs e)
		{
			_hasMouseFocus = true;

			Notification?.StopTimeout();
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			_hasMouseFocus = false;

			if (!_hasKeyboardFocus)
			{
				Notification?.StartTimeout();
			}
		}

		protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
		{
			_hasKeyboardFocus = true;

			Notification?.StopTimeout();
		}

		protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
		{
			_hasKeyboardFocus = false;

			if (!_hasMouseFocus)
			{
				Notification?.StartTimeout();
			}
		}
	}
}