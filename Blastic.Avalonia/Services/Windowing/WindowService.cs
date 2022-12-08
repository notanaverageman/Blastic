using Avalonia;
using Avalonia.Controls;
using Blastic.LifetimeManagement;
using Blastic.Ordering;
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

		if (window.IsLoaded)
		{
			window.Activate();
		}
		else
		{
			window.Show();
		}

		if (viewModel is IHasLifetime hasLifetime)
		{
			IDisposable? closeSubscription = null;

			closeSubscription = hasLifetime.Lifetime.Closure.Subscribe(() =>
			{
				window.Close();
				closeSubscription?.Dispose();
			}, Order.AbsoluteMaximum);

			hasLifetime.Lifetime.Activate();
		}
		else if (viewModel is IHasAsyncLifetime hasAsyncLifetime)
		{
			IDisposable? closeSubscription = null;

			closeSubscription = hasAsyncLifetime.Lifetime.Closure.Subscribe(() =>
			{
				window.Close();
				closeSubscription?.Dispose();

				return Task.CompletedTask;
			}, Order.AbsoluteMaximum);

			await hasAsyncLifetime.Lifetime.Activate();
		}
	}
}