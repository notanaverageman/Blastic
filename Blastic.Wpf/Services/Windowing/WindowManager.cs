using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Blastic.Commanding;
using Blastic.LifetimeManagement;
using Blastic.LifetimeManagement.Contexts;
using Blastic.Ordering;
using Blastic.Services.Windowing;
using Blastic.ViewManagement;
using ActivationContext = Blastic.LifetimeManagement.Contexts.ActivationContext;

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
				
				closeSubscription = hasLifetime.Lifetime.Close.Subscribe(x =>
				{
					window.Close();
					closeSubscription?.Dispose();

					return Task.CompletedTask;
				}, Order.AbsoluteMaximum);

				CommandContext<ActivationContext> commandContext = new CommandContext<ActivationContext>(
					new ActivationContext(),
					CancellationToken.None);

				await hasLifetime.Lifetime.Activate.Execute(commandContext);
			}
		}
	}
}