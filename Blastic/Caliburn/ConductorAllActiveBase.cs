using Blastic.Execution;
using Caliburn.Micro;

namespace Blastic.Caliburn
{
	public class ConductorAllActiveBase<T> : Conductor<T>.Collection.AllActive, IHasExecutionContext where T : class 
	{
		public ExecutionContextFactory ExecutionContextFactory { get; }
		public ExecutionContext ExecutionContext { get; }
		
		public ConductorAllActiveBase(ExecutionContextFactory executionContextFactory)
		{
			ExecutionContextFactory = executionContextFactory;
			ExecutionContext = executionContextFactory.Create();
		}
	}
}