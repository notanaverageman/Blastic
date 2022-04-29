using System;
using System.Resources;
using System.Threading;
using Blastic.DynamicControls;
using Blastic.Forms.DynamicControls;
using Blastic.Forms.Platform;
using Blastic.Forms.Services.Navigation;
using Blastic.Forms.Services.Settings;
using Blastic.Forms.UserInterface;
using Blastic.Forms.ViewManagement;
using Blastic.Ordering;
using Blastic.Platform;
using Blastic.Services.Localization;
using Blastic.Services.Messaging;
using Blastic.Services.Notifications;
using Blastic.Services.Settings;
using Blastic.ViewManagement;
using Blastic.ViewManagement.TypeMappers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xamarin.Forms;

namespace Blastic.Forms.Initialization
{
	public class BlasticApplicationBuilder
	{
		private readonly IServiceCollection _serviceCollection;

		public Type MainViewModelType { get; set; }
		public Action<Application> ApplicationRunner { get; set; }

		public BlasticApplicationBuilder(IServiceCollection serviceCollection)
		{
			_serviceCollection = serviceCollection;

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

		public BlasticApplicationBuilder UseApplicationRunner(Action<Application> applicationRunner)
		{
			ApplicationRunner = applicationRunner;
			return this;
		}

		public BlasticApplicationBuilder UseMainViewModel<T>() where T : class
		{
			MainViewModelType = typeof(T);
			return this;
		}

		public BlasticApplicationBuilder AddShellTab<T>() where T : IShellTab
		{
			RegisterType<IShellTab>(typeof(T));
			return this;
		}
		
		public BlasticApplicationBuilder AddLocalizationSource(ResourceManager resourceManager, Order order = null)
		{
			_serviceCollection.AddSingleton<ILocalizationSource>(new ResourceManagerLocalizationSource(resourceManager, order));
			return this;
		}

		public BlasticApplicationBuilder AddSettingsStorage()
		{
			_serviceCollection.AddSingleton<ISettingsStorage, SettingsStorage>();
			return this;
		}

		private void AddDefaults()
		{
			_serviceCollection.AddSingleton(SynchronizationContext.Current);
			_serviceCollection.AddSingleton<IPlatformSpecifics, FormsPlatformSpecifics>();
			_serviceCollection.AddSingleton<IViewLocator<VisualElement>, ViewLocator>();
			_serviceCollection.AddSingleton<ILocalizationService, LocalizationService>();
			_serviceCollection.AddSingleton<INotificationService, NotificationService>();
			_serviceCollection.AddSingleton<INavigationService, NavigationService>();
			_serviceCollection.AddSingleton<IEventAggregator, EventAggregator>();
			_serviceCollection.AddSingleton<IPresenterSource, PresenterSource>(_ => PresenterSource.Instance);

			_serviceCollection.AddSingleton<IHostLifetime, FormsHostLifetime>();

			AddTypeMapper(new SuffixTypeMapper("View", "ViewModel", Order.AbsoluteMaximum));
			AddTypeMapper(new InheritanceTypeMapper(typeof(DynamicModel), typeof(DynamicControl)));
		}

		private void RegisterType<T>(Type settingType) where T : class
		{
			_serviceCollection.AddSingleton(settingType);
			_serviceCollection.AddSingleton(y => (T)y.GetService(settingType));
		}
	}
}