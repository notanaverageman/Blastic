using System;
using System.Collections.Generic;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Blastic.DynamicControls;
using Blastic.Ordering;
using Blastic.Platform;
using Blastic.Services.Dialogs;
using Blastic.Services.Localization;
using Blastic.Services.Messaging;
using Blastic.Services.Notifications;
using Blastic.Settings;
using Blastic.ViewManagement;
using Blastic.ViewManagement.TypeMappers;
using Blastic.Wpf.DynamicControls;
using Blastic.Wpf.Initialization.Extensions;
using Blastic.Wpf.Localization;
using Blastic.Wpf.Platform;
using Blastic.Wpf.Services.Dialogs;
using Blastic.Wpf.Services.Windowing;
using Blastic.Wpf.UserInterface.Logs;
using Blastic.Wpf.ViewManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Blastic.Wpf.Initialization
{
	public class BlasticApplicationBuilder
	{
		private readonly IServiceCollection _serviceCollection;

		public bool StopHostOnApplicationShutdown { get; set; }
		public Type MainViewModelType { get; set; }

		public List<Func<DispatcherUnhandledExceptionEventArgs, Task>> UnhandledExceptionHandlers { get; }
		
		public BlasticApplicationBuilder(IServiceCollection serviceCollection)
		{
			_serviceCollection = serviceCollection;

			StopHostOnApplicationShutdown = true;
			UnhandledExceptionHandlers = new List<Func<DispatcherUnhandledExceptionEventArgs, Task>>();

			AddDefaults();
		}

		public BlasticApplicationBuilder AddTypeMapper(ITypeMapper typeMapper)
		{
			_serviceCollection.AddSingleton(typeMapper);
			return this;
		}

		public BlasticApplicationBuilder AddTypeMapper<TViewModel, TView>(Order? order = null)
		{
			AddTypeMapper(new InheritanceTypeMapper(typeof(TViewModel), typeof(TView), order));
			return this;
		}

		public BlasticApplicationBuilder UseApplication<T>() where T : Application
		{
			RegisterAsBaseAndSelf<Application, T>();
			return this;
		}

		public BlasticApplicationBuilder UseMainViewModel<T>() where T : class
		{
			MainViewModelType = typeof(T);
			return this;
		}

		public BlasticApplicationBuilder AddExceptionHandler(Func<DispatcherUnhandledExceptionEventArgs, Task> handler)
		{
			UnhandledExceptionHandlers.Add(handler);
			return this;
		}

		public BlasticApplicationBuilder AddSettingGroup<T>() where T : SettingGroup
		{
			RegisterAsBaseAndSelf<SettingGroup, T>();
			return this;
		}

		public BlasticApplicationBuilder AddLocalizationSource(ResourceManager resourceManager, Order? order = null)
		{
			_serviceCollection.AddSingleton<ILocalizationSource>(new ResourceManagerLocalizationSource(resourceManager, order));
			return this;
		}

		public BlasticApplicationBuilder AddLocalizationSource(ILocalizationSource source)
		{
			_serviceCollection.AddSingleton(source);
			return this;
		}

		public BlasticApplicationBuilder AddLogsWindow()
		{
			_serviceCollection.AddSingleton<UILogger>();
			_serviceCollection.AddSingleton<LogsViewModel>();
			_serviceCollection.AddSingleton<ILoggerProvider, UILoggerProvider>();

			_serviceCollection.AddLogging(x =>
			{
				x.AddFilter<UILoggerProvider>(_ => true);
			});

			return this;
		}

		public BlasticApplicationBuilder DontStopHostOnApplicationShutdown()
		{
			StopHostOnApplicationShutdown = false;
			return this;
		}

		private void AddDefaults()
		{
			_serviceCollection.AddSingleton<WpfApp>();
			_serviceCollection.AddSingleton<SynchronizationContext, DispatcherSynchronizationContext>(
				x =>
				{
					WpfApp app = x.GetRequiredService<WpfApp>();
					return new DispatcherSynchronizationContext(app.Dispatcher);
				});

			RegisterAsBaseAndSelf<IPlatformSpecifics, WpfPlatformSpecifics>();
			
			_serviceCollection.AddSingleton<IViewLocator<FrameworkElement>, ViewLocator>();
			_serviceCollection.AddSingleton<ILocalizationService, LocalizationService>();
			_serviceCollection.AddSingleton<INotificationService, NotificationService>();
			_serviceCollection.AddSingleton<IDialogService, DialogService>();
			_serviceCollection.AddSingleton<IWindowManager, WindowManager>();
			_serviceCollection.AddSingleton<IEventAggregator, EventAggregator>();
			_serviceCollection.AddSingleton<IPresenterSource, PresenterSource>(_ => PresenterSource.Instance);
			
			_serviceCollection.AddSingleton<LocalizableProperties>();
			_serviceCollection.AddSingleton<ILocalizationSource>(new LocalizationSource(Order.AbsoluteMaximum));
			
			AddTypeMapper(new SuffixTypeMapper("View", "ViewModel", Order.AbsoluteMaximum));
		}

		private void RegisterAsBaseAndSelf<TBase, TSelf>()
			where TBase : class
			where TSelf : class, TBase
		{
			_serviceCollection.AddSingleton<TSelf>();
			_serviceCollection.AddSingleton<TBase>(y => y.GetRequiredService<TSelf>());
		}
	}
}