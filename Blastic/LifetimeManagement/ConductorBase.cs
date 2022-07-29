using System;

namespace Blastic.LifetimeManagement
{
	/// <summary>
	/// A class with a lifecycle that can have multiple child objects whose lifecycles
	/// are managed by this class.
	/// </summary>
	/// <typeparam name="T">A type with a lifecycle.</typeparam>
	public class ConductorBase<T> : ConductorBaseCommon<T>, IHasLifetime where T : IHasLifetime
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
			return Lifetime.AddChildLifetime(item.Lifetime, LifetimeChainOptions);
		}
	}
}