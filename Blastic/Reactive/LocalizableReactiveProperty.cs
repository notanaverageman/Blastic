using Blastic.Services.Localization;
using System;

namespace Blastic.Reactive
{
	public class LocalizableReactiveProperty : ReactiveProperty<string>
	{
		public LocalizableReactiveProperty(ILocalizationService localizationService, string key)
		{
			localizationService.Culture.Subscribe(x =>
			{
				Value = localizationService.GetValue(key);
			});

			Value = localizationService.GetValue(key);
		}
	}
}