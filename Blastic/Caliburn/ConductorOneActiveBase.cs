using Blastic.Execution;
using Caliburn.Micro;

namespace Blastic.Caliburn
{
	public class ConductorOneActiveBase<T> : Conductor<T>.Collection.OneActive, IHasExecutionContext where T : class 
	{
		public ExecutionContextFactory ExecutionContextFactory { get; }
		public ExecutionContext ExecutionContext { get; }
		
		public ConductorOneActiveBase(ExecutionContextFactory executionContextFactory)
		{
			ExecutionContextFactory = executionContextFactory;
			ExecutionContext = executionContextFactory.Create();
		}
	}
}