using Avalonia;
using Blastic.Avalonia.Platform;
using Blastic.Avalonia.ViewManagement;
using Blastic.Platform;
using Blastic.Services.Localization;
using Blastic.Services.Messaging;
using Blastic.Services.Notifications;
using Blastic.ViewManagement;
using Depso;

namespace Blastic.Avalonia.Initialization;

[ServiceProviderModule]
public partial class BlasticServices
{
	private static void RegisterServices()
	{
		AddSingleton<IEventAggregator, EventAggregator>();
		AddSingleton<ILocalizationService, LocalizationService>();
		AddSingleton<INotificationService, NotificationService>();
		AddSingleton<IPlatformSpecifics, AvaloniaPlatformSpecifics>();
		AddSingleton<IViewLocator<StyledElement>, ViewLocator>();
	}
}