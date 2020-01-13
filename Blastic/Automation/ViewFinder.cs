using System.Windows;
using Blastic.ControlExtensions;
using Blastic.ViewManagement;

namespace Blastic.Automation
{
	public static partial class AutomationExtensions
	{
		public static FrameworkElement GetView(this IViewAware viewAware, object bindingSource)
		{
			FrameworkElement view = viewAware.View.Value;

			if (view == null)
			{
				return null;
			}

			return VisualTreeExtensions.FindChild(view, bindingSource);
		}
	}
}