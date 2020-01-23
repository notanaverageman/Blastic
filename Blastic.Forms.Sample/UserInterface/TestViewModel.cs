using System.Globalization;
using Blastic.Commanding;
using Blastic.Forms.UserInterface;
using Blastic.LifetimeManagement;
using Blastic.Ordering;
using Blastic.Reactive;
using Blastic.Services.Localization;

namespace Blastic.Forms.Sample.UserInterface
{
	public class TestViewModel : Screen, IShellTab
	{
		public Order Order { get; }
		public IReadOnlyReactiveProperty<string> Title { get; }

		public Command LanguageCommand { get; }

		public TestViewModel(ILocalizationService localizationService)
		{
			Order = new Order(0);

			Title = new LocalizableReactiveProperty(localizationService, "Sample.Homepage");

			bool x = false;

			LanguageCommand = new Command()
				.WithSubscribe(() =>
				{
					x = !x;
					localizationService.SetCulture(CultureInfo.GetCultureInfo(x ? "tr-TR" : "en-US"));
				});
		}
	}
}