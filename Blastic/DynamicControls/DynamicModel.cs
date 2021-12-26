using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Blastic.Commanding;
using Blastic.DynamicControls.Properties;
using Blastic.Reactive;

namespace Blastic.DynamicControls
{
	public class DynamicModel : ElementContainer
	{
		private readonly ConcurrentBag<TaskCompletionSource<bool>> _taskCompletionSources;

		public DynamicModel()
		{
			_taskCompletionSources = new ConcurrentBag<TaskCompletionSource<bool>>();
		}

		public void Ok()
		{
			Complete(true);
		}

		public void Cancel()
		{
			Complete(false);
		}

		public Task<bool> WaitCompletion()
		{
			TaskCompletionSource<bool> taskCompletionSource = new();
			_taskCompletionSources.Add(taskCompletionSource);

			return taskCompletionSource.Task;
		}

		private void Complete(bool result)
		{
			while(_taskCompletionSources.TryTake(out TaskCompletionSource<bool> taskCompletionSource))
			{
				if (taskCompletionSource.Task.IsCompleted)
				{
					continue;
				}

				taskCompletionSource.SetResult(result);
			}
		}
	}

	public static class DynamicModelExtensions
	{
		public static DynamicModel AddOkAction(
			this DynamicModel model,
			IObservable<bool>? canExecute = null,
			IReadOnlyReactiveProperty<string>? label = null)
		{
			return model.AddAction(model.Ok, canExecute, label, "OK");
		}

		public static DynamicModel AddCancelAction(
			this DynamicModel model,
			IObservable<bool>? canExecute = null,
			IReadOnlyReactiveProperty<string>? label = null)
		{
			return model.AddAction(model.Cancel, canExecute, label, "Cancel");
		}

		public static DynamicModel AddOkCancelAction(
			this DynamicModel model,
			IObservable<bool>? canExecuteOk = null,
			IObservable<bool>? canExecuteCancel = null,
			IReadOnlyReactiveProperty<string>? okLabel = null,
			IReadOnlyReactiveProperty<string>? cancelLabel = null)
		{
			return model.AddGroup(
				x =>
				{
					x.AddAction(model.Ok, canExecuteOk, okLabel, "OK");
					x.AddAction(model.Cancel, canExecuteCancel, cancelLabel, "Cancel");
					x.WithHorizontalAlignment(HorizontalAlignment.Right);
				});
		}

		private static T AddAction<T>(
			this T container,
			Action action,
			IObservable<bool>? canExecute,
			IReadOnlyReactiveProperty<string>? label,
			string fallbackLabel)
			where T : IElementContainer
		{
			AsyncCommand command = new(canExecute, action);

			container.AddAction(command, x =>
			{
				if (label != null)
				{
					x.WithLabel(label);
				}
				else
				{
					x.WithLabel(fallbackLabel);
				}
			});

			return container;
		}
	}
}