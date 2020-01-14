using System.Threading.Tasks;

namespace Blastic.DynamicControls
{
	public class DynamicModel : ElementContainer
	{
		private readonly TaskCompletionSource<bool> _taskCompletionSource;

		public DynamicModel()
		{
			_taskCompletionSource = new TaskCompletionSource<bool>();
		}

		public void Ok()
		{
			if (!_taskCompletionSource.Task.IsCompleted)
			{
				_taskCompletionSource.SetResult(true);
			}
		}

		public void Cancel()
		{
			if (!_taskCompletionSource.Task.IsCompleted)
			{
				_taskCompletionSource.SetResult(false);
			}
		}

		public Task<bool> WaitCompletion()
		{
			return _taskCompletionSource.Task;
		}
	}
}