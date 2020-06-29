using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Commanding;
using Blastic.DynamicControls;
using Blastic.LifetimeManagement;
using Blastic.LifetimeManagement.Contexts;

namespace Blastic.Services.Notifications
{
	public class Notification
	{
		private readonly Subject<bool> _hasFocus;

		public DynamicModel Model { get; }
		public TimeSpan ShowDuration { get; }

		public ILifetime Lifetime { get; }

		public AsyncCommand Dismiss { get; }

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

			Dismiss = Lifetime.Close.CanExecuteObservable
				.ToAsyncCommand()
				.WithSubscribe(DismissInternal);

			if (Model.MinWidth == 0)
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
						CommandContext<DeactivationContext> context = new CommandContext<DeactivationContext>(
							new DeactivationContext(),
							CancellationToken.None);

						await Lifetime.Deactivate.Execute(context);
					}
				});

			Lifetime.Activate.Subscribe(x =>
			{
				StartTimeout();
				return Task.CompletedTask;
			});
		}

		public void StartTimeout()
		{
			_hasFocus.OnNext(false);
		}

		public void StopTimeout()
		{
			_hasFocus.OnNext(true);
		}

		private async Task DismissInternal(CommandContext context)
		{
			CommandContext<ClosureContext> commandContext = new CommandContext<ClosureContext>(
				new ClosureContext(),
				context.CancellationToken);

			await Lifetime.Close.Execute(commandContext);
		}
	}
}