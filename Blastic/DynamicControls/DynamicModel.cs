using System.Collections.Concurrent;
using System.Threading.Tasks;

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
			TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
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
}