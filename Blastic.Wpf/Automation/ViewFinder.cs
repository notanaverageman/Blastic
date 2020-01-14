using System.Windows;
using Blastic.ControlExtensions;
using Blastic.ViewManagement;

namespace Blastic.Automation
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