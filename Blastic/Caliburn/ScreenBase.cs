using Blastic.Execution;
using Caliburn.Micro;

namespace Blastic.Caliburn
{
	public class ScreenBase : Screen, IHasExecutionContext
	{
		public ExecutionContext ExecutionContext { get; }
		public ExecutionContextFactory ExecutionContextFactory { get; }

		public ScreenBase(ExecutionContextFactory executionContextFactory)
		{
			ExecutionContextFactory = executionContextFactory;
			ExecutionContext = executionContextFactory.Create();
		}
	}
}