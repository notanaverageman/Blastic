using System.Threading.Tasks;
using Blastic.Reactive;

namespace Blastic.Services.Notifications
{
	public interface INotificationService
	{
		int MaximumActiveNotificationCount { get; set; }

		ReactiveCollection<Notification> Notifications { get; }
		ReactiveCollection<Notification> ActiveNotifications { get; }

		Task Enqueue(Notification notification);
		Task EnqueueWithoutNotifying(Notification notification);
	}
}