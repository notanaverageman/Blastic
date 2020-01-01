using System.Windows;
using Blastic.Execution;
using Blastic.Reactive;
using Blastic.ViewManagement;

namespace Blastic.LifetimeManagement
{
	public class Screen : IHasLifetime, IHasExecutionContext, IViewAware
	{
		public ExecutionContextFactory ExecutionContextFactory { get; }
		public ExecutionContext ExecutionContext { get; }

		public ILifetime Lifetime { get; }

		public IReactiveProperty<UIElement> View { get; }

		public Screen(ExecutionContextFactory executionContextFactory)
		{
			ExecutionContextFactory = executionContextFactory;
			ExecutionContext = executionContextFactory.Create();

			Lifetime = new Lifetime();
			View = new ReactiveProperty<UIElement>();
		}
	}
}