using System.Resources;
using Blastic.DynamicControls;
using Blastic.Forms.DynamicControls;
using Blastic.Forms.Properties;
using Blastic.Forms.UserInterface;
using Blastic.Initialization.Steps;
using Blastic.Ordering;
using Blastic.Services.Localization;
using Blastic.Services.Messaging;
using Blastic.Services.Notifications;
using Microsoft.Extensions.DependencyInjection;

namespace Blastic.Forms.Initialization.Extensions
{
	public static class ApplicationExtensions
	{
		public static BlasticApplication AddInitializationStep<T>(this BlasticApplication application) where T : class, IInitializationStep
		{
			return application.RegisterType<IInitializationStep, T>();
		}

		// TODO:
		//public static BlasticApplication AddSetting<T>(this BlasticApplication application) where T : class, ISettingsSectionViewModel
		//{
		//	return application.RegisterType<ISettingsSectionViewModel, T>();
		//}

		public static BlasticApplication AddShellTab<T>(this BlasticApplication application) where T : class, IShellTab
		{
			return application.RegisterType<IShellTab, T>();
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

		// TODO:
		//public static BlasticApplication AddSettingsWindow(this BlasticApplication application)
		//{
		//	return application.Configure(x =>
		//	{
		//		x.AddSingleton<SettingsViewModel>();
		//		x.AddSingleton<IInitializationStep, ReadSettingsStep>();
		//	});
		//}

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
				x.AddSingleton<IEventAggregator, EventAggregator>();
				x.AddSingleton<IPresenterSource, PresenterSource>(y => PresenterSource.Instance);
				// TODO:
				//x.AddSingleton<IDialogService, DialogService>();
				//x.AddSingleton<IWindowManager, WindowManager>();
			});
		}
	}
}