using Blastic.Commanding;

namespace Blastic.LifetimeManagement.Contexts
{
	public class ClosureContext : ICancellable
	{
		public bool? DialogResult { get; private set; }
		public bool IsCancelled => DialogResult == null;

		public ClosureContext(bool dialogResult = false)
		{
			DialogResult = dialogResult;
		}

		public void Cancel()
		{
			DialogResult = null;
		}
	}
}