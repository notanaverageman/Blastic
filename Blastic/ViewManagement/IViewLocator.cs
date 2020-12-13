namespace Blastic.ViewManagement
{
	/// <summary>
	/// Locates a view for given viewmodel.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IViewLocator<out T>
	{
		/// <summary>
		/// Return a view for given viewmodel.
		/// </summary>
		/// <remarks>
		/// An implementation should first check if the viewmodel implements <see cref="IViewAware"/>
		/// and returns its view if the view is not null.
		/// </remarks>
		/// <param name="model">The viewmodel.</param>
		/// <returns>A view for the given viewmodel.</returns>
		T Locate(object model);
	}
}