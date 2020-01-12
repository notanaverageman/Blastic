using System.Threading;

namespace Blastic.LifetimeManagement.Contexts
{
	public class ActivationContext
	{
		public CancellationToken CancellationToken { get; }

		public ActivationContext(CancellationToken cancellationToken)
		{
			CancellationToken = cancellationToken;
		}
	}
}