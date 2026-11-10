using System;
using System.Threading.Tasks;
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

		if (page.Window is Window existingWindow)
		{
			application.ActivateWindow(existingWindow);
			return;
		}

		Window window = new(page);
		WindowLifetime lifetime = WindowLifetimeBinding.Attach(window, viewModel);
		await lifetime.OnCreated();
		application.OpenWindow(window);
	}
}
