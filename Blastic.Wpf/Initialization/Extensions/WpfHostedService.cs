using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Blastic.Platform;
using Blastic.ViewManagement;
using Blastic.Wpf.Platform;
using Blastic.Wpf.Services.Windowing;
using Blastic.Wpf.ViewManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Blastic.Wpf.Initialization.Extensions
{
	public class WpfHostedService : IHostedService
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly BlasticApplicationBuilder _builder;

		private Thread _uiThread;

		public WpfHostedService(
			IServiceProvider serviceProvider,
			BlasticApplicationBuilder builder)
		{
			_serviceProvider = serviceProvider;
			_builder = builder;
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			_uiThread = new Thread(UiThreadStart);

			_uiThread.SetApartmentState(ApartmentState.STA);
			_uiThread.IsBackground = true;

			_uiThread.Start();

			return Task.CompletedTask;
		}

		public async Task StopAsync(CancellationToken cancellationToken)
		{
			Application application = _serviceProvider.GetRequiredService<Application>();

			if (application.Dispatcher.HasShutdownStarted)
			{
				return;
			}

			await application.Dispatcher.InvokeAsync(application.Shutdown);
		}

		private void UiThreadStart()
		{
			SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext());

			object mainViewModel = _serviceProvider.GetRequiredService(_builder.MainViewModelType);
			Application application = _serviceProvider.GetRequiredService<Application>();
			IWindowManager windowManager = _serviceProvider.GetRequiredService<IWindowManager>();
			IHostApplicationLifetime lifetime = _serviceProvider.GetRequiredService<IHostApplicationLifetime>();

			PlatformSpecifics.Current = new WpfPlatformSpecifics(application.Dispatcher);
			ViewLocator.Current = _serviceProvider.GetRequiredService<IViewLocator<FrameworkElement>>();

			foreach (Func<DispatcherUnhandledExceptionEventArgs, Task> handler in _builder.UnhandledExceptionHandlers)
			{
				application.DispatcherUnhandledException += async (sender, args) =>
				{
					await handler(args);
				};
			}

			application.Startup += async (sender, args) =>
			{
				ViewLocator.HookLoadedUnloadedEvents();

				await windowManager.ShowWindow(mainViewModel);
			};

			application.Exit += (sender, args) =>
			{
				if (_builder.StopHostOnApplicationShutdown)
				{
					lifetime.StopApplication();
				}
			};

			application.Run();
		}
	}
}