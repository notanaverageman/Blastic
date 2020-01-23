using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Blastic.ViewManagement;

namespace Blastic.Wpf.ViewManagement
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

		protected override void PostProcessCreatedView(FrameworkElement view, object model)
		{
			view.DataContext = model;
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