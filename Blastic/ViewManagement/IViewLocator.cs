using System.Windows;

namespace Blastic.ViewManagement
{
	public interface IViewLocator
	{
		UIElement Locate(object model);
	}
}