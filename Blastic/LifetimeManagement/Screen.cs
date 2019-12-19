using Blastic.Execution;

namespace Blastic.LifetimeManagement
{
	public class Screen : IHasLifetime, IHasExecutionContext
	{
		public ExecutionContextFactory ExecutionContextFactory { get; }
		public ExecutionContext ExecutionContext { get; }

		public ILifetime Lifetime { get; }

		public Screen(ExecutionContextFactory executionContextFactory)
		{
			ExecutionContextFactory = executionContextFactory;
			ExecutionContext = executionContextFactory.Create();

			Lifetime = new Lifetime();
		}
	}
}