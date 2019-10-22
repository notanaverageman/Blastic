using System.Threading.Tasks;

namespace Blastic.Controls.DynamicControls
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
			_taskCompletionSource.SetResult(true);
		}

		public void Cancel()
		{
			_taskCompletionSource.SetResult(false);
		}

		public Task<bool> WaitCompletion()
		{
			return _taskCompletionSource.Task;
		}
	}
}