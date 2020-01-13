namespace Blastic.ViewManagement
{
	public interface IViewLocator<out T>
	{
		T Locate(object model);
	}
}