using System.Windows;

namespace Blastic.ViewManagement
{
	public interface IViewLocator
	{
		FrameworkElement Locate(object model);
	}
}