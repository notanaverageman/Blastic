using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Blastic.LifetimeManagement;
using Blastic.Ordering;
using Blastic.ViewManagement;

namespace Blastic.Wpf.Services.Windowing
{
	public class WindowManager : IWindowManager
	{
		private readonly IViewLocator<FrameworkElement> _viewLocator;

		public WindowManager(IViewLocator<FrameworkElement> viewLocator)
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

			if (window.IsLoaded && new WindowInteropHelper(window).Handle != IntPtr.Zero)
			{
				window.Activate();
			}
			else
			{
				window.Show();
			}

			if (model is IHasLifetime hasLifetime)
			{
				IDisposable closeSubscription = null;
				
				closeSubscription = hasLifetime.Lifetime.Closure.Subscribe(() =>
				{
					window.Close();
					closeSubscription?.Dispose();

					return Task.CompletedTask;
				}, Order.AbsoluteMaximum);

				await hasLifetime.Lifetime.Activate();
			}
		}
	}
}