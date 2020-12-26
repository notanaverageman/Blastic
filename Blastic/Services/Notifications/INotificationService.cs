using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Blastic.Services.Notifications
{
	/// <summary>
	/// A service to show and manage notifications.
	/// </summary>
	public interface INotificationService
	{
		/// <summary>
		/// Maximum number of notifications that can be active. It is -1 by default.
		/// </summary>
		/// <remarks>
		/// If this property is 0, no notifications will be activated. If it is -1,
		/// then number of active notifications will have no limit.
		/// </remarks>
		int MaximumActiveNotificationCount { get; set; }

		/// <summary>
		/// Collection of all notifications. Notifications will be removed from
		/// this collection when they are deinitialized.
		/// </summary>
		ReadOnlyObservableCollection<Notification> Notifications { get; }

		/// <summary>
		/// Collection of active notifications. Notifications will be added to this
		/// collection when they are activated and removed from this collection when
		/// they are deactivated.
		/// </summary>
		ReadOnlyObservableCollection<Notification> ActiveNotifications { get; }

		/// <summary>
		/// Enqueue a new notification. If the number of active notifications
		/// is equal to <see cref="MaximumActiveNotificationCount"/> before enqueuing,
		/// the oldest notification will be deactivated.
		/// </summary>
		/// <param name="notification">Notification to enqueue.</param>
		/// <returns>a task to be awaited.</returns>
		Task Enqueue(Notification notification);

		/// <summary>
		/// Enqueue a new notification without activating it.
		/// </summary>
		/// <param name="notification">Notification to enqueue.</param>
		/// <returns>a task to be awaited.</returns>
		Task EnqueueWithoutNotifying(Notification notification);
	}
}