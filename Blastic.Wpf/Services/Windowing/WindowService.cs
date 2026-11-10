using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Blastic.Services.Windowing;
using Blastic.ViewManagement;

namespace Blastic.Wpf.Services.Windowing
{
	public class WindowService : IWindowService
	{
		private readonly IViewLocator<FrameworkElement> _viewLocator;

		public WindowService(IViewLocator<FrameworkElement> viewLocator)
		{
			_viewLocator = viewLocator;
		}

		public async Task ShowWindow(object model)
		{
			UIElement view = _viewLocator.Locate(model);

			if (!(view is Window window))
			{
				throw new ArgumentException($"Located view type is not a Window. Model type: {model.GetType()}, View type: {view.GetType()}");
			}

			window.DataContext = model;
			WindowLifetime lifetime = WindowLifetimeBinding.Attach(window, model);
			await lifetime.OnCreated();

			if (window.IsLoaded && new WindowInteropHelper(window).Handle != IntPtr.Zero)
			{
				window.Activate();
			}
			else
			{
				window.Show();
			}
		}
	}
}
