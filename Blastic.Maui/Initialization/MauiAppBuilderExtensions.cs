using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using Blastic.DynamicControls;
using Blastic.LifetimeManagement;
using Blastic.Maui.DynamicControls;
using Blastic.Maui.Platform;
using Blastic.Maui.Services.Navigation;
using Blastic.Maui.ViewManagement;
using Blastic.Ordering;
using Blastic.Platform;
using Blastic.Services.Localization;
using Blastic.Services.Messaging;
using Blastic.Services.Notifications;
using Blastic.Settings;
using Blastic.ViewManagement;
using Blastic.ViewManagement.TypeMappers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;

namespace Blastic.Maui.Initialization;

public static class MauiAppBuilderExtensions
{
	public static MauiAppBuilder UseBlastic<TApp, TMainViewModel>(this MauiAppBuilder builder)
		where TApp : Application, new()
		where TMainViewModel : class
	{
		AddDefaults(builder);

		builder.Services.AddSingleton<TApp>();
		builder.Services.AddSingleton<TMainViewModel>();
		builder.Services.AddSingleton(x => x.GetRequiredService<TApp>().Dispatcher);
		builder.Services.AddSingleton(_ => SynchronizationContext.Current!);

		builder.UseMauiApp(serviceProvider =>
		{
			PlatformSpecifics.Current = serviceProvider.GetRequiredService<IPlatformSpecifics>();
			ViewLocator.Current = serviceProvider.GetRequiredService<IViewLocator<VisualElement>>();

			TMainViewModel mainViewModel = serviceProvider.GetRequiredService<TMainViewModel>();
			TApp application = serviceProvider.GetRequiredService<TApp>();

			Page? mainPage = ViewLocator.Current.Locate(mainViewModel) as Page;
			application.MainPage = mainPage;

			SubscribeToLifecycleEvents(mainPage, mainViewModel);

			return application;
		});

		return builder;
	}

	public static MauiAppBuilder AddTypeMapper(this MauiAppBuilder builder, ITypeMapper typeMapper)
	{
		builder.Services.AddSingleton(typeMapper);
		return builder;
	}

	public static MauiAppBuilder AddTypeMapper<TViewModel, TView>(this MauiAppBuilder builder, Order? order = null)
	{
		builder.AddTypeMapper(new InheritanceTypeMapper(typeof(TViewModel), typeof(TView), order));
		return builder;
	}

	public static MauiAppBuilder AddLocalizationSource(this MauiAppBuilder builder, ResourceManager resourceManager, Order? order = null)
	{
		builder.Services.AddSingleton<ILocalizationSource>(new ResourceManagerLocalizationSource(resourceManager, order));
		return builder;
	}

	public static MauiAppBuilder AddLocalizationSource(this MauiAppBuilder builder, ILocalizationSource source)
	{
		builder.Services.AddSingleton(source);
		return builder;
	}

	public static MauiAppBuilder AddSettingSection<T>(this MauiAppBuilder builder) where T : SettingsSectionViewModel
	{
		builder.RegisterAsBaseAndSelf<SettingsSectionViewModel, T>();
		return builder;
	}

	private static void RegisterAsBaseAndSelf<TBase, TSelf>(this MauiAppBuilder builder)
		where TBase : class
		where TSelf : class, TBase
	{
		builder.Services.AddSingleton<TSelf>();
		builder.Services.AddSingleton<TBase>(y => y.GetRequiredService<TSelf>());
	}

	private static void AddDefaults(MauiAppBuilder builder)
	{
		builder.Services.AddSingleton<IPlatformSpecifics, MauiPlatformSpecifics>();
		builder.Services.AddSingleton<IViewLocator<VisualElement>, ViewLocator>();
		builder.Services.AddSingleton<ILocalizationService, LocalizationService>();
		builder.Services.AddSingleton<INotificationService, NotificationService>();
		builder.Services.AddSingleton<INavigationService, NavigationService>();
		builder.Services.AddSingleton<IEventAggregator, EventAggregator>();
		builder.Services.AddSingleton<IPresenterSource, PresenterSource>(_ => PresenterSource.Instance);

		builder.AddTypeMapper(new SuffixTypeMapper("View", "ViewModel", Order.AbsoluteMaximum));
		builder.AddTypeMapper(new InheritanceTypeMapper(typeof(DynamicModel), typeof(DynamicControl)));
	}

	private static void SubscribeToLifecycleEvents(Page? mainPage, object mainViewModel)
	{
		if (mainPage == null)
		{
			return;
		}

		mainPage.ParentChanged += (_, _) =>
		{
			if (mainPage.Parent is not Window window)
			{
				return;
			}

			ILifetime? lifetime = (mainViewModel as IHasLifetime)?.Lifetime;
			IAsyncLifetime? asyncLifetime = (mainViewModel as IHasAsyncLifetime)?.Lifetime;

			window.Created += async (_, _) =>
			{
				lifetime?.Initialize();
				await (asyncLifetime?.Initialize() ?? Task.CompletedTask);
			};

			window.Activated += async (_, _) =>
			{
				lifetime?.Activate();
				await (asyncLifetime?.Activate() ?? Task.CompletedTask);
			};

			window.Deactivated += async (_, _) =>
			{
				lifetime?.Deactivate();
				await (asyncLifetime?.Deactivate() ?? Task.CompletedTask);
			};

			window.Stopped += async (_, _) =>
			{
				lifetime?.Close();
				await (asyncLifetime?.Close() ?? Task.CompletedTask);
			};
		};
	}
}