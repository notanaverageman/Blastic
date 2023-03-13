using Avalonia;
using Blastic.Avalonia.Platform;
using Blastic.Avalonia.ViewManagement;
using Blastic.Platform;
using Blastic.Services.Localization;
using Blastic.Services.Messaging;
using Blastic.Services.Notifications;
using Blastic.ViewManagement;
using Jab;

namespace Blastic.Avalonia.Initialization;

[Singleton<IEventAggregator, EventAggregator>]
[Singleton<ILocalizationService, LocalizationService>]
[Singleton<INotificationService, NotificationService>]
[Singleton<IPlatformSpecifics, AvaloniaPlatformSpecifics>]
[Singleton<IViewLocator<StyledElement>, ViewLocator>]

[ServiceProviderModule]
public interface IBlasticServices
{
}