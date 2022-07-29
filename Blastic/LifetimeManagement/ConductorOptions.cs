namespace Blastic.LifetimeManagement
{
	/// <summary>
	/// Class to manage the behavior of a <see cref="ConductorBase{T}"/>.
	/// </summary>
	public class ConductorOptions
	{
		/// <summary>
		/// Remove all children when the object is deinitialized.
		/// </summary>
		public bool ClearItemsOnClosure { get; }

		/// <summary>
		/// Create a new instance of <see cref="ConductorOptions"/>.
		/// </summary>
		/// <param name="clearItemsOnClosure">Remove all children when the object is closed.</param>
		public ConductorOptions(bool clearItemsOnClosure = false)
		{
			ClearItemsOnClosure = clearItemsOnClosure;
		}
	}
}