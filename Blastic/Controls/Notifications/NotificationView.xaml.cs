using System.Windows;
using System.Windows.Input;
using Blastic.Services.Notifications;

namespace Blastic.Controls.Notifications
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

		public NotificationView()
		{
			InitializeComponent();
		}

		protected override void OnMouseEnter(MouseEventArgs e)
		{
			Notification?.StopTimeout();
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			Notification?.StartTimeout();
		}
	}
}