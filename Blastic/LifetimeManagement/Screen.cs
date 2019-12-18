using Blastic.Execution;
using Blastic.Reactive;

namespace Blastic.LifetimeManagement
{
	public class Screen : IHasLifetime, IHasExecutionContext
	{
		public ExecutionContextFactory ExecutionContextFactory { get; }
		public ExecutionContext ExecutionContext { get; }

		public ILifetime Lifetime { get; }

		public IReactiveProperty<string> DisplayName { get; }

		public Screen(ExecutionContextFactory executionContextFactory)
		{
			ExecutionContextFactory = executionContextFactory;
			ExecutionContext = executionContextFactory.Create();

			Lifetime = new Lifetime();

			DisplayName = new ReactiveProperty<string>(GetType().ToString());
		}
	}
}