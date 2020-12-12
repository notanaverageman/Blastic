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
		public bool ClearItemsOnDeinitialize { get; }

		/// <summary>
		/// Create a new instance.
		/// </summary>
		/// <param name="clearItemsOnDeinitialize">Remove all children when the object is deinitialized.</param>
		public ConductorOptions(bool clearItemsOnDeinitialize = false)
		{
			ClearItemsOnDeinitialize = clearItemsOnDeinitialize;
		}
	}
}