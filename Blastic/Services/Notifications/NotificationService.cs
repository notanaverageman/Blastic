using System.Threading.Tasks;
using Blastic.Ordering;
using Blastic.Reactive;

namespace Blastic.Services.Notifications
{
	/// <summary>
	/// Default implementation of <see cref="INotificationService"/>.
	/// </summary>
	public class NotificationService : INotificationService
	{
		/// <inheritdoc />
		public int MaximumActiveNotificationCount { get; set; }

		/// <inheritdoc />
		public ReactiveCollection<Notification> Notifications { get; }

		/// <inheritdoc />
		public ReactiveCollection<Notification> ActiveNotifications { get; }

		/// <summary>
		/// Creates a new instance of <see cref="NotificationService"/>.
		/// </summary>
		public NotificationService()
		{
			Notifications = new ReactiveCollection<Notification>();
			ActiveNotifications = new ReactiveCollection<Notification>();

			MaximumActiveNotificationCount = -1;
		}

		/// <inheritdoc />
		public async Task Enqueue(Notification notification)
		{
			await EnqueueWithoutNotifying(notification);

			if (MaximumActiveNotificationCount == 0)
			{
				return;
			}

			ActiveNotifications.Add(notification);

			if (MaximumActiveNotificationCount > -1 && ActiveNotifications.Count > MaximumActiveNotificationCount)
			{
				ActiveNotifications.RemoveAt(0);
			}

			await notification.Lifetime.Activate();
		}

		/// <inheritdoc />
		public Task EnqueueWithoutNotifying(Notification notification)
		{
			Notifications.Add(notification);

			notification.Lifetime.Deactivation.Subscribe(() =>
			{
				ActiveNotifications.Remove(notification);
			}, Order.AbsoluteMaximum);

			notification.Lifetime.Closure.Subscribe(() =>
			{
				Notifications.Remove(notification);
				return Task.CompletedTask;
			}, Order.AbsoluteMaximum);

			return Task.CompletedTask;
		}
	}
}