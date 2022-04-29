namespace Blastic.LifetimeManagement
{
	/// <summary>
	/// An interface to show that the class has lifecycle.
	/// </summary>
	public interface IHasAsyncLifetime
	{
		/// <summary>
		/// Lifetime of the object.
		/// </summary>
		IAsyncLifetime Lifetime { get; }
	}
}