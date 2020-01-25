using Blastic.Commanding;
using Blastic.Forms.UserInterface;
using Blastic.LifetimeManagement;
using Blastic.Ordering;
using Blastic.Reactive;
using Blastic.Services.Localization;
using Blastic.Services.Settings;

namespace Blastic.Forms.Sample.UserInterface
{
	public class TestViewModel : Screen, IShellTab
	{
		public Order Order { get; }
		public IReadOnlyReactiveProperty<string> Title { get; }

		public IReactiveProperty<string> Text { get; }

		public AsyncCommand WriteSettingCommand { get; }

		public TestViewModel(
			ILocalizationService localizationService,
			ISettingsService settingsService)
		{
			Order = new Order(0);
			Title = new LocalizableReactiveProperty(localizationService, "Sample.Homepage");

			Text = new ReactiveProperty<string>();

			WriteSettingCommand = new AsyncCommand()
				.WithSubscribe(async () =>
				{
					await settingsService.Put("Sample.Text", Text.Value);
				});

			Lifetime.Activate.Subscribe(async x =>
			{
				Text.Value = await settingsService.Get<string>("Sample.Text");
			});
		}
	}
}