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

[Singleton(typeof(IViewLocator<StyledElement>), typeof(ViewLocator))]
[Singleton(typeof(IPlatformSpecifics), typeof(AvaloniaPlatformSpecifics))]
[Singleton(typeof(ILocalizationService), typeof(LocalizationService))]
[Singleton(typeof(INotificationService), typeof(NotificationService))]
[Singleton(typeof(IEventAggregator), typeof(EventAggregator))]

[ServiceProviderModule]
public interface IBlasticServices
{
}