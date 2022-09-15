using System;
using System.Threading.Tasks;
using Blastic.LifetimeManagement;
using Blastic.Ordering;
using Blastic.Services.Windowing;
using Blastic.ViewManagement;
using Microsoft.Maui.Controls;

namespace Blastic.Maui.Services.Windowing;

public class WindowService : IWindowService
{
	private readonly IViewLocator<VisualElement> _viewLocator;

	public WindowService(IViewLocator<VisualElement> viewLocator)
	{
		_viewLocator = viewLocator;
	}

	public async Task ShowWindow(object viewModel)
	{
		Application? application = Application.Current;

		if (application == null)
		{
			throw new InvalidOperationException("Can't get current application.");
		}

		VisualElement view = _viewLocator.Locate(viewModel);

		if (view is not Page page)
		{
			throw new ArgumentException($"Located view type is not a Page. Model type: {viewModel.GetType()}, View type: {view.GetType()}");
		}

		page.BindingContext = viewModel;

		Window window = new(page);
		application.OpenWindow(window);

		if (viewModel is IHasLifetime hasLifetime)
		{
			IDisposable? closeSubscription = null;

			closeSubscription = hasLifetime.Lifetime.Closure.Subscribe(() =>
			{
				application.CloseWindow(window);
				closeSubscription?.Dispose();
			}, Order.AbsoluteMaximum);

			hasLifetime.Lifetime.Activate();
		}
		else if (viewModel is IHasAsyncLifetime hasAsyncLifetime)
		{
			IDisposable? closeSubscription = null;

			closeSubscription = hasAsyncLifetime.Lifetime.Closure.Subscribe(() =>
			{
				application.CloseWindow(window);
				closeSubscription?.Dispose();

				return Task.CompletedTask;
			}, Order.AbsoluteMaximum);

			await hasAsyncLifetime.Lifetime.Activate();
		}
	}
}