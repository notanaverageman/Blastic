using System.Resources;
using Blastic.DynamicControls;
using Blastic.Initialization.Steps;
using Blastic.Ordering;
using Blastic.Wpf.Properties;
using Blastic.Services.Dialog;
using Blastic.Services.Localization;
using Blastic.Services.Messaging;
using Blastic.Services.Notifications;
using Blastic.Services.Settings;
using Blastic.Services.Windowing;
using Blastic.UserInterface.Settings;
using Blastic.UserInterface.Settings.Steps;
using Blastic.Wpf.DynamicControls;
using Blastic.Wpf.Services.Dialog;
using Blastic.Wpf.Services.Settings;
using Blastic.Wpf.Services.Windowing;
using Blastic.Wpf.UserInterface.Logs;
using Blastic.Wpf.UserInterface.Logs.Settings;
using Blastic.Wpf.UserInterface.TabbedMain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Blastic.Wpf.Initialization.Extensions
{
	public static class ApplicationExtensions
	{
		public static BlasticApplication AddInitializationStep<T>(this BlasticApplication application) where T : class, IInitializationStep
		{
			return application.RegisterType<IInitializationStep, T>();
		}

		public static BlasticApplication AddSetting<T>(this BlasticApplication application) where T : class, ISettingsSectionViewModel
		{
			return application.RegisterType<ISettingsSectionViewModel, T>();
		}

		public static BlasticApplication AddMainTab<T>(this BlasticApplication application) where T : class, IMainTab
		{
			return application.RegisterType<IMainTab, T>();
		}

		private static BlasticApplication RegisterType<T, TImplementation>(this BlasticApplication application)
			where T : class
			where TImplementation : class, T
		{
			return application
				.Configure(x => x.AddSingleton<TImplementation>())
				.Configure(x => x.AddSingleton<T, TImplementation>(y => y.GetService<TImplementation>()));
		}

		public static BlasticApplication AddLocalizationSource(
			this BlasticApplication application,
			ResourceManager resourceManager,
			Order order = null)
		{
			return application.Configure(x =>
			{
				x.AddSingleton<ILocalizationSource>(new ResourceManagerLocalizationSource(resourceManager, order));
			});
		}

		public static BlasticApplication AddLogsWindow(this BlasticApplication application)
		{
			return application
				.Configure(x =>
				{
					x.AddSingleton(UILogger.Instance);
					x.AddSingleton<LogsViewModel>();
					x.AddSingleton<LogSettingsViewModel>();
					
					x.AddLogging(y =>
					{
						y.AddProvider(new UILoggerProvider());
						y.AddFilter<UILoggerProvider>(_ => true);
					});
				})
				.AddSetting<LogSettingsViewModel>();
		}

		public static BlasticApplication AddSettingsService(this BlasticApplication application)
		{
			return application.Configure(x =>
			{
				x.AddSingleton<SettingsViewModel>();
				x.AddSingleton<ISettingsService, SettingsService>();
				x.AddSingleton<IInitializationStep, ReadSettingsStep>();
			});
		}

		internal static BlasticApplication AddDefaults(this BlasticApplication application)
		{
			return application
				.AddDefaultServices()
				.AddLocalizationSource(Resources.ResourceManager, Order.AbsoluteMaximum);
		}

		private static BlasticApplication AddDefaultServices(this BlasticApplication application)
		{
			return application.Configure(x =>
			{
				x.AddSingleton<ILocalizationService, LocalizationService>();
				x.AddSingleton<INotificationService, NotificationService>();
				x.AddSingleton<IDialogService, DialogService>();
				x.AddSingleton<IWindowManager, WindowManager>();
				x.AddSingleton<IEventAggregator, EventAggregator>();
				x.AddSingleton<IPresenterSource, PresenterSource>(y => PresenterSource.Instance);
			});
		}
	}
}