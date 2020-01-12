using System.Threading;
using System.Threading.Tasks;
using Blastic.Common;
using Blastic.LifetimeManagement.Contexts;
using Blastic.Reactive;

namespace Blastic.Services.Notifications
{
	public class NotificationService : INotificationService
	{
		public int MaximumActiveNotificationCount { get; set; }

		public ReactiveCollection<Notification> Notifications { get; }
		public ReactiveCollection<Notification> ActiveNotifications { get; }

		public NotificationService()
		{
			Notifications = new ReactiveCollection<Notification>();
			ActiveNotifications = new ReactiveCollection<Notification>();
		}

		public async Task Enqueue(Notification notification)
		{
			await EnqueueWithoutNotifying(notification);
			ActiveNotifications.Add(notification);

			if (MaximumActiveNotificationCount > 0 && ActiveNotifications.Count > MaximumActiveNotificationCount)
			{
				ActiveNotifications.RemoveAt(ActiveNotifications.Count - 1);
			}

			await notification.Lifetime.Activate.Execute(new ActivationContext(CancellationToken.None));
		}

		public Task EnqueueWithoutNotifying(Notification notification)
		{
			Notifications.Add(notification);

			notification.Lifetime.Deactivate.Subscribe(x =>
			{
				ActiveNotifications.Remove(notification);
				return Task.CompletedTask;
			}, Order.AbsoluteMaximum);

			notification.Lifetime.Close.Subscribe(x =>
			{
				Notifications.Remove(notification);
				return Task.CompletedTask;
			}, Order.AbsoluteMaximum);

			return Task.CompletedTask;
		}
	}
}