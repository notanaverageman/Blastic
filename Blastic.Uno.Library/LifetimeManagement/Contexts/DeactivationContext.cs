using System.Threading;

namespace Blastic.LifetimeManagement.Contexts
{
	public class DeactivationContext
	{
		public CancellationToken CancellationToken { get; }

		public DeactivationContext(CancellationToken cancellationToken)
		{
			CancellationToken = cancellationToken;
		}
	}
}