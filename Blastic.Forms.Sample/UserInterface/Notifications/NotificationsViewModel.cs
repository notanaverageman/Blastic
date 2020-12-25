using System;
using System.Reactive.Linq;
using Blastic.DynamicControls;
using Blastic.DynamicControls.Properties;
using Blastic.Forms.Sample.Controls.Overlay;
using Blastic.Reactive;
using Blastic.Services.Notifications;
using DynamicData.Binding;

namespace Blastic.Forms.Sample.UserInterface.Notifications
{
	public class NotificationsViewModel
	{
		private int _test;

		public INotificationService NotificationService { get; }

		public IReactiveProperty<OverlayState> State { get; }

		public NotificationsViewModel(INotificationService notificationService)
		{
			NotificationService = notificationService;

			State = new ReactiveProperty<OverlayState>();

			NotificationService.ActiveNotifications
				.ToObservableChangeSet()
				.Select(_ => NotificationService.ActiveNotifications.Count)
				.Subscribe(
					x =>
					{
						if (x == 0)
						{
							State.Value = OverlayState.Invisible;
						}
					});
		}

		public void Show()
		{
			_test++;
			DynamicModel model = new();
			Notification notification = new(model, TimeSpan.FromSeconds(5000));

			model
				.AddGroup(
					x =>
					{
						x.AddLabel(
							"Some notification text => " + _test,
							y => y.WithHorizontalAlignment(HorizontalAlignment.Stretch));

						x.AddGroup(
							y =>
							{
								y.AddAction(async () => await notification.Dismiss.Execute(), z => z.WithLabel("OK"));
								y.WithHorizontalAlignment(HorizontalAlignment.Right);
							});

						x.WithMargin(new Thickness(0));
					});

			NotificationService.Enqueue(notification);

			State.Value = OverlayState.Collapsed;
		}
	}
}