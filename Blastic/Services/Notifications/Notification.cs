using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Blastic.Commanding;
using Blastic.DynamicControls;
using Blastic.LifetimeManagement;
using Blastic.Reactive;

namespace Blastic.Services.Notifications
{
	/// <summary>
	/// Notification class holds a <see cref="DynamicModel"/> to show on UI and a
	/// <see cref="Command"/> that can be executed to dismiss it. It can also be
	/// automatically dismissed after some time.
	/// </summary>
	public class Notification
	{
		private readonly Subject<bool> _hasFocus;

		/// <summary>
		/// Model that is used to create the user interface.
		/// </summary>
		public DynamicModel Model { get; }

		/// <summary>
		/// The duration that when passed, the notification is dismissed.
		/// </summary>
		public TimeSpan ShowDuration { get; }

		/// <summary>
		/// Lifetime of this notification.
		/// </summary>
		public ILifetime Lifetime { get; }

		/// <summary>
		/// Command that dismisses the notification when executed.
		/// </summary>
		public Command Dismiss { get; }

		/// <summary>
		/// Creates a new instance with given message and an optional show duration.
		/// </summary>
		/// <param name="text">Message to show.</param>
		/// <param name="showDuration">Duration to show this notification.</param>
		/// <param name="dismissOnTimeout">Whether to dismiss the notification when duration passes.</param>
		public Notification(
			string text,
			TimeSpan? showDuration = null,
			bool dismissOnTimeout = true)
			:
			this(new DynamicModel().AddLabel(text), showDuration, dismissOnTimeout)
		{
		}

		/// <summary>
		/// Creates a new instance with given message and an optional show duration.
		/// </summary>
		/// <param name="text">Message to show.</param>
		/// <param name="showDuration">Duration to show this notification.</param>
		/// <param name="dismissOnTimeout">Whether to dismiss the notification when duration passes.</param>
		public Notification(
			IReadOnlyReactiveProperty<string> text,
			TimeSpan? showDuration = null,
			bool dismissOnTimeout = true)
			:
			this(new DynamicModel().AddLabel(text), showDuration, dismissOnTimeout)
		{
		}

		/// <summary>
		/// Creates a new instance with given model and an optional show duration.
		/// </summary>
		/// <param name="model">Message to show.</param>
		/// <param name="showDuration">Duration to show this notification.</param>
		/// <param name="dismissOnTimeout">Whether to dismiss the notification when duration passes.</param>
		public Notification(
			DynamicModel model,
			TimeSpan? showDuration = null,
			bool dismissOnTimeout = true)
		{
			Model = model;
			Lifetime = new Lifetime();

			ShowDuration = showDuration ?? TimeSpan.FromSeconds(3);

			if (ShowDuration < TimeSpan.Zero)
			{
				throw new ArgumentOutOfRangeException(nameof(showDuration));
			}

			Dismiss = Lifetime.Closure.CanExecuteObservable
				.ToCommand()
				.WithSubscribe(DismissInternal);

			if (Math.Abs(Model.MinWidth) < 1)
			{
				Model.MinWidth = 400;
			}

			_hasFocus = new Subject<bool>();

			// Throttle will ensure that an event will be fired only if there is
			// no event after it within ShowDuration.
			// It will trigger deactivation if the value is false which means it
			// does not have focus.
			_hasFocus
				.Throttle(ShowDuration)
				.Where(x => x == false)
				.FirstAsync()
				.Subscribe(async x =>
				{
					if (dismissOnTimeout)
					{
						await Dismiss.Execute();
					}
					else
					{
						await Lifetime.Deactivate();
					}
				});

			Lifetime.Activation.Subscribe(() =>
			{
				StartTimeout();
				return Task.CompletedTask;
			});
		}

		/// <summary>
		/// Call this function to start auto dismissal timer when the notification
		/// loses focus.
		/// </summary>
		/// <remarks>
		/// Timer is started on activation by default. This method is
		/// used for enabling dismissal when notification loses focus.
		/// </remarks>
		public void StartTimeout()
		{
			_hasFocus.OnNext(false);
		}

		/// <summary>
		/// Call this function to stop auto dismissal timer when the notification
		/// captures focus.
		/// </summary>
		/// <remarks>
		/// Timer is started on activation by default. This method is
		/// used for disabling dismissal when notification is focused.
		/// </remarks>
		public void StopTimeout()
		{
			_hasFocus.OnNext(true);
		}

		private async Task DismissInternal()
		{
			await Lifetime.Close();
		}
	}
}