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
using Jab;
using Microsoft.Maui.Controls;

namespace Blastic.Maui.Initialization;

[Singleton<IDialogService, DialogService>]
[Singleton<IEventAggregator, EventAggregator>]
[Singleton<ILocalizationService, LocalizationService>]
[Singleton<INavigationService, NavigationService>]
[Singleton<INotificationService, NotificationService>]
[Singleton<IPlatformSpecifics, MauiPlatformSpecifics>]
[Singleton<IPresenterSource>(Factory = nameof(CreatePresenterSource))]
[Singleton<ITypeMapper, InheritanceTypeMapper<DynamicModel, DynamicControl>>]
[Singleton<IViewLocator<VisualElement>, ViewLocator>]
[Singleton<IWindowService, WindowService>]

[ServiceProviderModule]
public interface IBlasticServices
{
	static IPresenterSource CreatePresenterSource() => PresenterSource.Instance;
}