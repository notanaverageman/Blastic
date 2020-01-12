using Windows.UI.Xaml;

namespace Blastic.ViewManagement
{
	public interface IViewLocator
	{
		FrameworkElement Locate(object model);
	}
}