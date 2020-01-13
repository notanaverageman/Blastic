using Blastic.Execution;
using Blastic.Reactive;
using Blastic.ViewManagement;

namespace Blastic.LifetimeManagement
{
	public class Screen : IHasLifetime, IViewAware
	{
		public ExecutionContext ExecutionContext { get; }

		public ILifetime Lifetime { get; }
		public IReactiveProperty<object> View { get; }

		public Screen()
		{
			ExecutionContext = new ExecutionContext();

			Lifetime = new Lifetime();
			View = new ReactiveProperty<object>();
		}
	}
}