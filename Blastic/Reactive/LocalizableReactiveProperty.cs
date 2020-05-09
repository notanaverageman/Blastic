using System.Reactive.Linq;
using Blastic.Services.Localization;

namespace Blastic.Reactive
{
	public class LocalizableReactiveProperty : ReadOnlyReactiveProperty<string>
	{
		public LocalizableReactiveProperty(
			ILocalizationService localizationService,
			string key)
			:
			base(
				localizationService.Culture.Select(x => localizationService.GetValue(key)),
				localizationService.GetValue(key))
		{
		}
	}
}