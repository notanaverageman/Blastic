using System.Collections.ObjectModel;
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
		private readonly ObservableCollection<Notification> _notifications;
		private readonly ObservableCollection<Notification> _activeNotifications;
		
		/// <inheritdoc />
		public int MaximumActiveNotificationCount { get; set; }

		/// <inheritdoc />
		public ReadOnlyObservableCollection<Notification> Notifications { get; }

		/// <inheritdoc />
		public ReadOnlyObservableCollection<Notification> ActiveNotifications { get; }

		/// <summary>
		/// Creates a new instance of <see cref="NotificationService"/>.
		/// </summary>
		public NotificationService()
		{
			_notifications = new ReactiveCollection<Notification>();
			_activeNotifications = new ReactiveCollection<Notification>();
			
			Notifications = new ReadOnlyObservableCollection<Notification>(_notifications);
			ActiveNotifications = new ReadOnlyObservableCollection<Notification>(_activeNotifications);

			MaximumActiveNotificationCount = -1;
		}

		/// <inheritdoc />
		public async Task Enqueue(Notification notification)
		{
			await EnqueueWithoutNotifying(notification);

			if (MaximumActiveNotificationCount != 0)
			{
				_activeNotifications.Add(notification);
			}

			if (MaximumActiveNotificationCount > -1 && ActiveNotifications.Count > MaximumActiveNotificationCount)
			{
				_activeNotifications.RemoveAt(0);
			}

			await notification.Lifetime.Activate();
		}

		/// <inheritdoc />
		public Task EnqueueWithoutNotifying(Notification notification)
		{
			_notifications.Add(notification);

			notification.Lifetime.Deactivation.Subscribe(() =>
			{
				_activeNotifications.Remove(notification);
			}, Order.AbsoluteMaximum);

			notification.Lifetime.Closure.Subscribe(() =>
			{
				_notifications.Remove(notification);
				return Task.CompletedTask;
			}, Order.AbsoluteMaximum);

			return Task.CompletedTask;
		}
	}
}