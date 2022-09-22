using Blastic.ViewManagement;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace Blastic.Maui.ControlExtensions;

public static class FocusExtensions
{
	public static void SetFocus(this IViewAware viewAware, object bindingSource)
	{
		if (viewAware.View.Value is not IVisualTreeElement view)
		{
			return;
		}

		VisualElement? element = VisualTreeExtensions.FindChild(view, bindingSource);

		element?.Dispatcher.Dispatch(() =>
		{
			element.Focus();
		});
	}
}