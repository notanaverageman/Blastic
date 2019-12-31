using System.Windows;
using Blastic.Services.Notifications;

namespace Blastic.Controls.Notifications
{
	public partial class NotificationAreaView
	{
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

		public NotificationAreaView()
		{
			InitializeComponent();
		}
	}
}