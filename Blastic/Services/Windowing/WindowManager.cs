using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Blastic.Common;
using Blastic.LifetimeManagement;
using Blastic.ViewManagement;
using ActivationContext = Blastic.LifetimeManagement.Contexts.ActivationContext;

namespace Blastic.Services.Windowing
{
	public class WindowManager : IWindowManager
	{
		private readonly IViewLocator _viewLocator;

		public WindowManager(IViewLocator viewLocator)
		{
			_viewLocator = viewLocator;
		}

		public async Task ShowWindow(object model, Action<Window> configure)
		{
			UIElement view = _viewLocator.Locate(model);

			if (!(view is Window window))
			{
				throw new ArgumentException($"Located view type is not a Window. Model type: {model.GetType()}, View type: {view.GetType()}");
			}

			window.DataContext = model;
			configure?.Invoke(window);

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
				
				closeSubscription = hasLifetime.Lifetime.Close.Subscribe(x =>
				{
					window.Close();
					closeSubscription?.Dispose();

					return Task.CompletedTask;
				}, Order.AbsoluteMaximum);

				ActivationContext context = new ActivationContext(CancellationToken.None);
				await hasLifetime.Lifetime.Activate.Execute(context);
			}
		}
	}
}