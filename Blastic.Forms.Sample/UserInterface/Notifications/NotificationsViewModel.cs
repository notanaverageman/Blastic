using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Blastic.DynamicControls;
using Blastic.DynamicControls.Properties;
using Blastic.Forms.Sample.Controls.Overlay;
using Blastic.Platform;
using Blastic.Reactive;
using Blastic.Services.Notifications;
using Blastic.ViewManagement;
using DynamicData.Binding;
using Xamarin.Forms;
using Animation = Blastic.Animations.Animation;
using Thickness = Blastic.DynamicControls.Properties.Thickness;

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

			State = new ReactiveProperty<OverlayState>(OverlayState.Invisible);

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
			ViewAwareNotification notification = new(model, TimeSpan.FromSeconds(10));

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

		public class ViewAwareNotification : Notification, IViewAware
		{
			public IReactiveProperty<object?> View { get; }
			
			public ViewAwareNotification(
				DynamicModel model,
				TimeSpan? showDuration = null,
				bool dismissOnTimeout = true)
				:
				base(model, showDuration, dismissOnTimeout)
			{
				View = new ReactiveProperty<object?>(default);

				Lifetime.Activation.Subscribe(
					() =>
					{
						if (View.Value is not VisualElement view)
						{
							return;
						}
						
						IObservable<double> animation = Animation.Create(TimeSpan.FromMilliseconds(300));

						animation
							.ObserveOnUI()
							.Subscribe(
								x =>
								{
									view.Opacity = x;
								});
					});

				Lifetime.Deactivation.Subscribe(
					() =>
					{
						TaskCompletionSource<bool> taskCompletionSource = new();
						
						if (View.Value is not VisualElement view)
						{
							return Task.CompletedTask;
						}
						
						IObservable<double> animation = Animation.Create(TimeSpan.FromMilliseconds(300));

						animation
							.ObserveOnUI()
							.Subscribe(
								x =>
								{
									view.Opacity = 1 - x;
								},
								() => taskCompletionSource.SetResult(true));

						return taskCompletionSource.Task;
					});
			}
		}
	}
}