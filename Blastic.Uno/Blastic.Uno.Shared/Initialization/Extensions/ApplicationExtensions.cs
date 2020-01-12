using System.Resources;
using Windows.ApplicationModel.Resources;
using Autofac;
using Blastic.Common;
using Blastic.Execution;
using Blastic.Initialization.Steps;
using Blastic.Services.Localization;
using Blastic.Services.Messaging;
using Blastic.Services.Notifications;
using Blastic.UserInterface.Logs;
using Blastic.UserInterface.Logs.Settings;
using Blastic.UserInterface.Settings;
using Blastic.UserInterface.TabbedMain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Blastic.Initialization.Extensions
{
	public static class ApplicationExtensions
	{
		public static BlasticApplication RegisterInitializationStepsAssembly<T>(this BlasticApplication application)
		{
			return application.RegisterTypes<T, IInitializationStep>();
		}

		public static BlasticApplication RegisterSettingsAssembly<T>(this BlasticApplication application)
		{
			return application.RegisterTypes<T, ISettingsSectionViewModel>();
		}

		public static BlasticApplication RegisterMainTabs<T>(this BlasticApplication application)
		{
			return application.RegisterTypes<T, IMainTab>();
		}

		private static BlasticApplication RegisterTypes<TAssembly, TBase>(this BlasticApplication application)
		{
			return application.Configure(builder =>
			{
				builder
					.RegisterAssemblyTypes(typeof(TAssembly).Assembly)
					.AssignableTo<TBase>()
					.AsImplementedInterfaces()
					.AsSelf()
					.SingleInstance();
			});
		}

		public static BlasticApplication AddLocalizationSource(
			this BlasticApplication application,
			ResourceManager resourceManager,
			Order order = null)
		{
			return application.Configure(builder =>
			{
				builder
					.RegisterInstance(new ResourceManagerLocalizationSource(resourceManager, order))
					.AsImplementedInterfaces();
			});
		}

		public static BlasticApplication AddLocalizationSource(
			this BlasticApplication application,
			ResourceLoader resourceLoader,
			Order order = null)
		{
			return application.Configure(builder =>
			{
				builder
					.RegisterInstance(new ResourceLoaderLocalizationSource(resourceLoader, order))
					.AsImplementedInterfaces();
			});
		}

		public static BlasticApplication AddLogsWindow(this BlasticApplication application)
		{
			return application.Configure(builder =>
			{
				builder
					.RegisterType<LogsViewModel>()
					.SingleInstance();

				builder
					.RegisterType<LogSink>()
					.SingleInstance();

				builder
					.RegisterType<LogSettingsViewModel>()
					.SingleInstance()
					.AsImplementedInterfaces()
					.AsSelf();
			});
		}

		public static BlasticApplication AddSettingsWindow(this BlasticApplication application)
		{
			return application.Configure(builder =>
			{
				builder
					.RegisterType<SettingsViewModel>()
					.SingleInstance();

				builder.RegisterType<ReadSettingsStep>()
					.SingleInstance()
					.As<IInitializationStep>();
			});
		}

		internal static BlasticApplication AddDefaults(this BlasticApplication application)
		{
			return application
				.AddLogging()
				.AddDefaultServices();
		}

		private static BlasticApplication AddLogging(this BlasticApplication application, LogLevel minimumLogLevel = LogLevel.Trace)
		{
			return application.Configure(x =>
			{
				x.AddLogging(builder =>
				{
					builder.SetMinimumLevel(minimumLogLevel);
					builder.AddSerilog();
				});
			});
		}

		private static BlasticApplication AddDefaultServices(this BlasticApplication application)
		{
			return application.Configure(builder =>
			{
				builder
					.RegisterType<ExecutionContextFactory>()
					.SingleInstance();

				builder
					.RegisterType<LocalizationService>()
					.As<ILocalizationService>()
					.SingleInstance();

				builder
					.RegisterType<NotificationService>()
					.As<INotificationService>()
					.SingleInstance();

				builder
					.RegisterType<EventAggregator>()
					.As<IEventAggregator>()
					.SingleInstance();
			});
		}
	}
}