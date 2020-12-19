using System;
using System.Collections.Generic;
using System.Resources;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Blastic.Data;
using Blastic.DynamicControls;
using Blastic.Ordering;
using Blastic.Services.Localization;
using Blastic.Services.Messaging;
using Blastic.Services.Notifications;
using Blastic.Services.Settings;
using Blastic.ViewManagement;
using Blastic.ViewManagement.TypeMappers;
using Blastic.Wpf.Data.Initialization.Steps;
using Blastic.Wpf.Data.ProgramData;
using Blastic.Wpf.Data.ProgramData.Migrations;
using Blastic.Wpf.DynamicControls;
using Blastic.Wpf.Initialization.Steps;
using Blastic.Wpf.Properties;
using Blastic.Wpf.Services.Dialog;
using Blastic.Wpf.Services.Windowing;
using Blastic.Wpf.UserInterface.Logs;
using Blastic.Wpf.UserInterface.Logs.Settings;
using Blastic.Wpf.UserInterface.Settings;
using Blastic.Wpf.UserInterface.TabbedMain;
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

		public BlasticApplicationBuilder AddTypeMapper<TViewModel, TView>(Order order = null)
		{
			AddTypeMapper(new InheritanceTypeMapper(typeof(TViewModel), typeof(TView), order));
			return this;
		}

		public BlasticApplicationBuilder UseApplication<T>() where T : Application
		{
			RegisterType<Application>(typeof(T));
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

		public BlasticApplicationBuilder AddSetting<T>() where T : ISettingsSectionViewModel
		{
			RegisterType<ISettingsSectionViewModel>(typeof(T));
			return this;
		}

		public BlasticApplicationBuilder AddMainTab<T>() where T : IMainTab
		{
			RegisterType<IMainTab>(typeof(T));
			return this;
		}

		public BlasticApplicationBuilder AddInitializationStep<T>() where T : class, IInitializationStep
		{
			RegisterType<IInitializationStep>(typeof(T));
			return this;
		}

		public BlasticApplicationBuilder AddLocalizationSource(ResourceManager resourceManager, Order order = null)
		{
			_serviceCollection.AddSingleton<ILocalizationSource>(new ResourceManagerLocalizationSource(resourceManager, order));
			return this;
		}

		public BlasticApplicationBuilder AddSettingsService<T>() where T : class, ISettingsStorage
		{
			_serviceCollection.AddSingleton<SettingsViewModel>();
			_serviceCollection.AddSingleton<ISettingsStorage, T>();
			_serviceCollection.AddSingleton<IInitializationStep, ReadSettingsStep>();

			return this;
		}

		public BlasticApplicationBuilder AddProgramDatabase(DatabaseProvider databaseProvider, string connectionString)
		{
			DatabaseConfiguration databaseConfiguration = new DatabaseConfiguration(databaseProvider, connectionString);

			_serviceCollection.AddSingleton(y => databaseConfiguration);
			_serviceCollection.AddSingleton<ConnectionFactory>();
			_serviceCollection.AddSingleton<ProgramDatabase>();
			_serviceCollection.AddSingleton<ProgramDatabaseMigrationBase, CreateSettingsTable>();
			_serviceCollection.AddSingleton<IInitializationStep, MigrateProgramDatabaseStep>();

			return this;
		}

		public BlasticApplicationBuilder AddLogsWindow()
		{
			_serviceCollection.AddSingleton(UILogger.Instance);
			_serviceCollection.AddSingleton<LogsViewModel>();

			_serviceCollection.AddLogging(y =>
			{
				y.AddProvider(new UILoggerProvider());
				y.AddFilter<UILoggerProvider>(_ => true);
			});

			AddSetting<LogSettingsViewModel>();

			return this;
		}

		public BlasticApplicationBuilder DontStopHostOnApplicationShutdown()
		{
			StopHostOnApplicationShutdown = false;
			return this;
		}

		private void AddDefaults()
		{
			_serviceCollection.AddSingleton<IViewLocator<FrameworkElement>, ViewLocator>();
			_serviceCollection.AddSingleton<ILocalizationService, LocalizationService>();
			_serviceCollection.AddSingleton<INotificationService, NotificationService>();
			_serviceCollection.AddSingleton<IDialogService, DialogService>();
			_serviceCollection.AddSingleton<IWindowManager, WindowManager>();
			_serviceCollection.AddSingleton<IEventAggregator, EventAggregator>();
			_serviceCollection.AddSingleton<IPresenterSource, PresenterSource>(y => PresenterSource.Instance);

			// TODO: Uncomment these and remove the line below when following issue is resolved:
			// https://github.com/dotnet/wpf/issues/3404
			//_serviceCollection.AddSingleton<LocalizableProperties>();
			//_serviceCollection.AddSingleton<ILocalizationSource>(new LocalizationSource(Order.AbsoluteMaximum));

			AddLocalizationSource(Resources.ResourceManager, Order.AbsoluteMaximum);

			AddTypeMapper<ISettingsSectionViewModel, FormSettingSectionView>(new Order(int.MaxValue));
			AddTypeMapper<SettingsViewModel, SettingsView>(new Order(int.MaxValue));
			AddTypeMapper(new SuffixTypeMapper("View", "ViewModel", Order.AbsoluteMaximum));
		}

		private void RegisterType<T>(Type settingType) where T : class
		{
			_serviceCollection.AddSingleton(settingType);
			_serviceCollection.AddSingleton(y => (T)y.GetService(settingType));
		}
	}
}