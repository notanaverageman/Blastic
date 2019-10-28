using System.Threading;

namespace Blastic.LifetimeManagement.Contexts
{
	public class InitializationContext
	{
		public CancellationToken CancellationToken { get; }

		public InitializationContext(CancellationToken cancellationToken)
		{
			CancellationToken = cancellationToken;
		}
	}
}