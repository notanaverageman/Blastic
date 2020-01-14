using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace Blastic.ViewManagement
{
	public class ViewLocator : ViewLocatorBase<FrameworkElement>
	{
		protected override void PostProcessAttachView(FrameworkElement view, IViewAware viewAware)
		{
			view.Unloaded += (sender, args) =>
			{
				viewAware.View.Value = null;
			};
		}

		protected override FrameworkElement PostProcessCachedView(FrameworkElement view)
		{
			if (!(view is Window window))
			{
				return view;
			}

			if (window.IsLoaded && new WindowInteropHelper(window).Handle != IntPtr.Zero)
			{
				return view;
			}

			return null;
		}

		protected override FrameworkElement CreateNotFoundView(Type type, string message)
		{
			return new TextBlock
			{
				Text = message
			};
		}
	}
}