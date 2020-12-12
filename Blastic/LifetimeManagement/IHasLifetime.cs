namespace Blastic.LifetimeManagement
{
	/// <summary>
	/// An interface to show that the class has lifecycle.
	/// </summary>
	public interface IHasLifetime
	{
		/// <summary>
		/// Lifetime of the object.
		/// </summary>
		ILifetime Lifetime { get; }
	}
}