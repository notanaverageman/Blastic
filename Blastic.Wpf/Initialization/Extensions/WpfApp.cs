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
	public class WpfApp
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly BlasticApplicationBuilder _builder;

		private readonly ManualResetEvent _dispatcherSetEvent;
		private readonly ManualResetEvent _startEvent;

		public Dispatcher Dispatcher { get; private set; }

		public WpfApp(IServiceProvider serviceProvider, BlasticApplicationBuilder builder)
		{
			_serviceProvider = serviceProvider;
			_builder = builder;

			_dispatcherSetEvent = new ManualResetEvent(false);
			_startEvent = new ManualResetEvent(false);

			Thread uiThread = new(UiThreadStart);

			uiThread.SetApartmentState(ApartmentState.STA);
			uiThread.IsBackground = true;

			uiThread.Start();

			_dispatcherSetEvent.WaitOne();
		}

		public void Start()
		{
			_startEvent.Set();
		}

		public async Task Stop()
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
			Dispatcher = Dispatcher.CurrentDispatcher;
			_dispatcherSetEvent.Set();
			
			_startEvent.WaitOne();
			
			SynchronizationContext synchronizationContext = _serviceProvider.GetRequiredService<SynchronizationContext>();
			SynchronizationContext.SetSynchronizationContext(synchronizationContext);

			PlatformSpecifics.Current = _serviceProvider.GetRequiredService<WpfPlatformSpecifics>();

			Application application = _serviceProvider.GetRequiredService<Application>();
			object mainViewModel = _serviceProvider.GetRequiredService(_builder.MainViewModelType);
			IWindowManager windowManager = _serviceProvider.GetRequiredService<IWindowManager>();
			IHostApplicationLifetime lifetime = _serviceProvider.GetRequiredService<IHostApplicationLifetime>();

			ViewLocator.Current = _serviceProvider.GetRequiredService<IViewLocator<FrameworkElement>>();

			foreach (Func<DispatcherUnhandledExceptionEventArgs, Task> handler in _builder.UnhandledExceptionHandlers)
			{
				application.DispatcherUnhandledException += async (_, args) =>
				{
					await handler(args);
				};
			}

			application.Startup += async (_, _) =>
			{
				ViewLocator.HookLoadedUnloadedEvents();

				await windowManager.ShowWindow(mainViewModel);
			};

			application.Exit += (_, _) =>
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