using System.Threading;

namespace Blastic.LifetimeManagement.Contexts
{
	public class ClosureContext
	{
		public CancellationToken CancellationToken { get; }
		public bool CanClose { get; set; }
		public bool? DialogResult { get; set; }

		public ClosureContext(CancellationToken cancellationToken)
		{
			CancellationToken = cancellationToken;
			CanClose = true;
		}
	}
}