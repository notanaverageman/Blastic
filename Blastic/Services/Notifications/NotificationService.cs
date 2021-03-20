using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Blastic.Ordering;
using Blastic.Platform;
using DynamicData;

namespace Blastic.Services.Notifications
{
	/// <summary>
	/// Default implementation of <see cref="INotificationService"/>.
	/// </summary>
	public class NotificationService : INotificationService
	{
		private readonly SourceList<Notification> _notifications;
		private readonly SourceList<Notification> _activeNotifications;
		
		/// <inheritdoc />
		public int MaximumActiveNotificationCount { get; set; }

		/// <inheritdoc />
		public ReadOnlyObservableCollection<Notification> Notifications { get; }

		/// <inheritdoc />
		public ReadOnlyObservableCollection<Notification> ActiveNotifications { get; }

		/// <summary>
		/// Creates a new instance of <see cref="NotificationService"/>.
		/// </summary>
		/// <param name="platformSpecifics">Platform specifics to access the UI thread.</param>
		public NotificationService(IPlatformSpecifics platformSpecifics)
		{
			_notifications = new SourceList<Notification>();
			_activeNotifications = new SourceList<Notification>();

			MaximumActiveNotificationCount = int.MaxValue;
			
			_notifications
				.Connect()
				.ObserveOnUI(platformSpecifics)
				.Bind(out ReadOnlyObservableCollection<Notification> notifications)
				.Subscribe();

			_activeNotifications
				.Connect()
				.ObserveOnUI(platformSpecifics)
				.Bind(out ReadOnlyObservableCollection<Notification> activeNotifications)
				.Subscribe();

			Notifications = notifications;
			ActiveNotifications = activeNotifications;
		}

		/// <inheritdoc />
		public async Task Enqueue(Notification notification)
		{
			await EnqueueWithoutNotifying(notification);

			if (MaximumActiveNotificationCount <= 0)
			{
				return;
			}

			if (ActiveNotifications.Count >= MaximumActiveNotificationCount)
			{
				Notification? first = _activeNotifications.Items.FirstOrDefault();

				if (first != null)
				{
					await first.Lifetime.Deactivate();
				}
			}

			_activeNotifications.Add(notification);
			await notification.Lifetime.Activate();
		}

		/// <inheritdoc />
		public async Task EnqueueWithoutNotifying(Notification notification)
		{
			_notifications.Add(notification);

			notification.Lifetime.Deactivation.Subscribe(() =>
			{
				_activeNotifications.Remove(notification);
			}, Order.AbsoluteMaximum);

			notification.Lifetime.Closure.Subscribe(() =>
			{
				_notifications.Remove(notification);
			}, Order.AbsoluteMaximum);
			
			await notification.Lifetime.Initialize();
		}
	}
}