using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Blastic.Services.Windowing;
using Blastic.ViewManagement;

namespace Blastic.Avalonia.Services.Windowing;

public class WindowService : IWindowService
{
	private readonly IViewLocator<StyledElement> _viewLocator;

	public WindowService(IViewLocator<StyledElement> viewLocator)
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

		StyledElement view = _viewLocator.Locate(viewModel);

		if (view is not Window window)
		{
			throw new ArgumentException($"Located view type is not a Window. Model type: {viewModel.GetType()}, View type: {view.GetType()}");
		}

		window.DataContext = viewModel;
		WindowLifetime lifetime = WindowLifetimeBinding.Attach(window, viewModel);
		await lifetime.OnCreated();

		if (window.IsLoaded)
		{
			window.Activate();
		}
		else if (window.WindowStartupLocation == WindowStartupLocation.CenterOwner
			&& application.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
			&& desktop.MainWindow is Window owner
			&& owner != window
			&& owner.IsVisible)
		{
			window.Show(owner);
		}
		else
		{
			window.Show();
		}
	}
}