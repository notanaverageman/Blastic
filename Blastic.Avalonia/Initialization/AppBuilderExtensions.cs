using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Blastic.Avalonia.ViewManagement;
using Blastic.LifetimeManagement;
using Blastic.Platform;
using Blastic.ViewManagement;
using Microsoft.Extensions.DependencyInjection;

namespace Blastic.Avalonia.Initialization;

public static class AppBuilderExtensions
{
	public static AppBuilder UseBlastic<TMainViewModel>(this AppBuilder builder, IServiceProvider services)
		where TMainViewModel : class
	{
		builder.AfterPlatformServicesSetup(_ =>
		{
			PlatformSpecifics.Current = services.GetRequiredService<IPlatformSpecifics>();
			ViewLocator.Current = services.GetRequiredService<IViewLocator<StyledElement>>();
		});

		builder.AfterSetup(_ =>
		{
			TMainViewModel mainViewModel = services.GetRequiredService<TMainViewModel>();

			IApplicationLifetime? applicationLifetime = builder.Instance?.ApplicationLifetime;

			if (applicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
			{
				if (ViewLocator.Current.Locate(mainViewModel) is not Window mainWindow)
				{
					throw new ArgumentException("The resolved view for main viewmodel is not a Window.");
				}

				SubscribeToLifecycleEvents(mainWindow, mainViewModel);

				desktop.MainWindow = mainWindow;
			}
			else if (applicationLifetime is ISingleViewApplicationLifetime singleView)
			{
				if (ViewLocator.Current.Locate(mainViewModel) is not Control control)
				{
					throw new ArgumentException("The resolved view for main viewmodel is not a Control.");
				}

				SubscribeToLifecycleEvents(control, mainViewModel);

				singleView.MainView = control;
			}
		});

		return builder;
	}

	private static void SubscribeToLifecycleEvents(Window mainWindow, object mainViewModel)
	{
		ILifetime? lifetime = (mainViewModel as IHasLifetime)?.Lifetime;
		IAsyncLifetime? asyncLifetime = (mainViewModel as IHasAsyncLifetime)?.Lifetime;

		mainWindow.Initialized += async (_, _) =>
		{
			lifetime?.Initialize();
			await (asyncLifetime?.Initialize() ?? Task.CompletedTask);
		};

		mainWindow.Loaded += async (_, _) =>
		{
			lifetime?.Activate();
			await (asyncLifetime?.Activate() ?? Task.CompletedTask);
		};

		mainWindow.Unloaded += async (_, _) =>
		{
			lifetime?.Deactivate();
			await (asyncLifetime?.Deactivate() ?? Task.CompletedTask);
		};

		mainWindow.Closed += async (_, _) =>
		{
			lifetime?.Close();
			await (asyncLifetime?.Close() ?? Task.CompletedTask);
		};
	}

	private static void SubscribeToLifecycleEvents(Control control, object mainViewModel)
	{
		ILifetime? lifetime = (mainViewModel as IHasLifetime)?.Lifetime;
		IAsyncLifetime? asyncLifetime = (mainViewModel as IHasAsyncLifetime)?.Lifetime;

		control.Loaded += async (_, _) =>
		{
			lifetime?.Activate();
			await (asyncLifetime?.Activate() ?? Task.CompletedTask);
		};

		control.Unloaded += async (_, _) =>
		{
			lifetime?.Close();
			await (asyncLifetime?.Close() ?? Task.CompletedTask);
		};
	}
}