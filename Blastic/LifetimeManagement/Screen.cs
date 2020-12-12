using Blastic.Execution;
using Blastic.Reactive;
using Blastic.ViewManagement;

namespace Blastic.LifetimeManagement
{
	/// <summary>
	/// A class that implements <see cref="IHasLifetime"/> and <see cref="IViewAware"/>.
	/// </summary>
	public class Screen : IHasLifetime, IViewAware
	{
		public ExecutionContext ExecutionContext { get; }

		/// <summary>
		/// Lifetime of the object.
		/// </summary>
		public ILifetime Lifetime { get; }

		/// <summary>
		/// An observable property that holds the view that this object is bound to.
		/// </summary>
		public IReactiveProperty<object?> View { get; }

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public Screen()
		{
			ExecutionContext = new ExecutionContext();

			Lifetime = new Lifetime();
			View = new ReactiveProperty<object?>();
		}
	}
}