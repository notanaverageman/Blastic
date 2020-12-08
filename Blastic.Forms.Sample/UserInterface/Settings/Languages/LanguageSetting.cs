using System;
using System.Globalization;
using Blastic.DynamicControls;
using Blastic.Forms.Sample.Resources;
using Blastic.Services.Localization;
using Blastic.Services.Settings;
using Blastic.Settings;

namespace Blastic.Forms.Sample.UserInterface.Settings.Languages
{
	public sealed class LanguageSetting : SelectionSetting<Language>
	{
		public LanguageSetting(
			ISettingsService settingsService,
			IPresenterSource presenterSource,
			ILocalizationService localizationService,
			LocalizableProperties localizableProperties)
			:
			base(
				settingsService,
				presenterSource,
				"Sample.Language",
				Language.System,
				new[]
				{
					Language.System,
					Language.English,
					Language.Turkish
				})
		{
			Element.WithLabel(localizableProperties.SampleSettingsLanguage);

			ReactiveSettingValue.Subscribe(
				x =>
				{
					localizationService.Culture = x switch
					{
						Language.English => CultureInfo.GetCultureInfo("en-US"),
						Language.Turkish => CultureInfo.GetCultureInfo("tr-TR"),
						Language.System => CultureInfo.InstalledUICulture,
						_ => throw new ArgumentOutOfRangeException(nameof(x), x, null)
					};
				});
		}
	}
}