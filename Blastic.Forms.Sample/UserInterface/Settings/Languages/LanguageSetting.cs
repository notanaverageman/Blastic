using System;
using System.Globalization;
using Blastic.DynamicControls;
using Blastic.DynamicControls.Elements;
using Blastic.Forms.Sample.Resources;
using Blastic.Reactive;
using Blastic.Services.Localization;
using Blastic.Services.Settings;
using Blastic.Settings;

namespace Blastic.Forms.Sample.UserInterface.Settings.Languages
{
	public sealed class LanguageSetting : SelectionSetting<Language>
	{
		public LanguageSetting(
			ISettingsStorage settingsStorage,
			IPresenterSource presenterSource,
			ILocalizationService localizationService,
			LocalizableProperties localizableProperties)
			:
			base(
				settingsStorage,
				presenterSource,
				"Sample.Language",
				Language.System,
				new SelectionValueWithLabel<Language>[]
				{
					new(GetName(localizableProperties, Language.System), Language.System),
					new(GetName(localizableProperties, Language.English), Language.English),
					new(GetName(localizableProperties, Language.Turkish), Language.Turkish)
				})
		{
			Element.WithLabel(localizableProperties.Settings.Language);

			ReactiveSettingValue.Subscribe(
				x =>
				{
					CultureInfo culture = x switch
					{
						Language.English => CultureInfo.GetCultureInfo("en-US"),
						Language.Turkish => CultureInfo.GetCultureInfo("tr-TR"),
						Language.System => CultureInfo.InstalledUICulture,
						_ => throw new ArgumentOutOfRangeException(nameof(x), x, null)
					};

					localizationService.ChangeCultureCommand.Execute(culture.DisplayName);
				});

			SaveOnChange = true;
		}
		
		private static IReadOnlyReactiveProperty<string> GetName(
			LocalizableProperties localizableProperties,
			Language language)
		{
			return language switch
			{
				Language.System => localizableProperties.Common.System,
				Language.English => localizableProperties.Settings.Language.English,
				Language.Turkish => localizableProperties.Settings.Language.Turkish,
				_ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
			};
		}
	}
}