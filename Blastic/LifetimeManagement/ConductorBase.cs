using System;
using System.Reactive.Disposables;
using System.Threading;

namespace Blastic.LifetimeManagement
{
	/// <summary>
	/// A class with a lifecycle that can have multiple child objects whose lifecycles
	/// are managed by this class.
	/// </summary>
	/// <typeparam name="T">A type with a lifecycle.</typeparam>
	public abstract class ConductorBase<T> : ConductorBaseCommon<T>, IHasLifetime where T : IHasLifetime
	{
		/// <inheritdoc />
		public ILifetime Lifetime { get; }
		
		/// <summary>
		/// Creates a new instance with default options.
		/// </summary>
		/// <param name="conductorOptions">The conductor options.</param>
		/// <param name="lifetimeChainOptions">The lifetime options for children.</param>
		public ConductorBase(
			ConductorOptions? conductorOptions = null,
			LifetimeChainOptions? lifetimeChainOptions = null)
			:
			base(conductorOptions, lifetimeChainOptions)
		{
			Lifetime = new Lifetime();
			SubscribeToLifetimeClosure(Lifetime);
		}

		protected override IDisposable AddChildLifetime(T item)
		{
			IDisposable closure = item.Lifetime.Closure.Subscribe((context, cancellationToken) =>
			{
				Close(item, context?.Result == true, cancellationToken);
			});

			IDisposable childLifetime = Lifetime.AddChildLifetime(item.Lifetime, LifetimeChainOptions);

			return new CompositeDisposable(closure, childLifetime);
		}

		/// <summary>
		/// Close the given item and remove it from children.
		/// </summary>
		/// <param name="item">The item to close.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="result">The result of the closure operation.</param>
		public abstract void Close(
			T item,
			bool result = false,
			CancellationToken cancellationToken = default);
	}
}