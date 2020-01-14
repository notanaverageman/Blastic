using System.Windows;
using Blastic.ViewManagement;
using Blastic.Wpf.ControlExtensions;

namespace Blastic.Wpf.Automation
{
	public static partial class AutomationExtensions
	{
		public static FrameworkElement GetView(this IViewAware viewAware, object bindingSource)
		{
			if (!(viewAware.View.Value is FrameworkElement view))
			{
				return null;
			}

			return VisualTreeExtensions.FindChild(view, bindingSource);
		}
	}
}