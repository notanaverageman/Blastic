using Avalonia;
using Blastic.Avalonia.DynamicControls;
using Blastic.Avalonia.Platform;
using Blastic.Avalonia.ViewManagement;
using Blastic.DynamicControls;
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
		AddSingleton<IPresenterSource>(_ => PresenterSource.Instance);
		AddSingleton<IViewLocator<StyledElement>, ViewLocator>();
	}
}