using System.Windows;
using System.Windows.Controls;
using Blastic.Services.Notifications;

namespace Blastic.Wpf.Controls
{
	public class NotificationAreaView : Control
	{
		static NotificationAreaView()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(NotificationAreaView), new FrameworkPropertyMetadata(typeof(NotificationAreaView)));
		}

		public static readonly DependencyProperty NotificationServiceProperty = DependencyProperty.Register(
			nameof(NotificationServiceProperty).Replace("Property", ""),
			typeof(INotificationService),
			typeof(NotificationAreaView),
			new PropertyMetadata(default(INotificationService)));
		public INotificationService NotificationService
		{
			get => (INotificationService)GetValue(NotificationServiceProperty);
			set => SetValue(NotificationServiceProperty, value);
		}
	}
}