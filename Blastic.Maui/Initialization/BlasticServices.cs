using Blastic.DynamicControls;
using Blastic.Maui.DynamicControls;
using Blastic.Maui.Platform;
using Blastic.Maui.Services.Dialogs;
using Blastic.Maui.Services.Navigation;
using Blastic.Maui.Services.Windowing;
using Blastic.Maui.ViewManagement;
using Blastic.Platform;
using Blastic.Services.Dialogs;
using Blastic.Services.Localization;
using Blastic.Services.Messaging;
using Blastic.Services.Notifications;
using Blastic.Services.Windowing;
using Blastic.ViewManagement;
using Blastic.ViewManagement.TypeMappers;
using Depso;
using Microsoft.Maui.Controls;

namespace Blastic.Maui.Initialization;

[ServiceProviderModule]
public partial class BlasticServices
{
	private static void RegisterServices()
	{
		AddSingleton<IDialogService, DialogService>();
		AddSingleton<IEventAggregator, EventAggregator>();
		AddSingleton<ILocalizationService, LocalizationService>();
		AddSingleton<INavigationService, NavigationService>();
		AddSingleton<INotificationService, NotificationService>();
		AddSingleton<IPlatformSpecifics, MauiPlatformSpecifics>();
		AddSingleton<IPresenterSource>(_ => PresenterSource.Instance);
		AddSingleton<ITypeMapper, InheritanceTypeMapper<DynamicModel, DynamicControl>>();
		AddSingleton<IViewLocator<VisualElement>, ViewLocator>();
		AddSingleton<IWindowService, WindowService>();
	}
}