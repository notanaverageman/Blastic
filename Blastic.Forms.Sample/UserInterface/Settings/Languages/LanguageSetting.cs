using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Blastic.DynamicControls;
using Blastic.Forms.Sample.Resources;
using Blastic.Reactive;
using Blastic.Services.Localization;
using Blastic.Services.Settings;
using Blastic.Settings;

namespace Blastic.Forms.Sample.UserInterface.Settings.Languages
{
	public sealed class LanguageSetting : SelectionSetting<LocalizedSettingValue<Language>>
	{
		private readonly LocalizableProperties _localizableProperties;

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
				new LocalizedSettingValue<Language>(Language.System, GetName(localizableProperties, Language.System)),
				new LocalizedSettingValue<Language>[]
				{
					new(Language.System, GetName(localizableProperties, Language.System)),
					new(Language.English, GetName(localizableProperties, Language.English)),
					new(Language.Turkish, GetName(localizableProperties, Language.Turkish))
				})
		{
			_localizableProperties = localizableProperties;
			
			Element.WithLabel(localizableProperties.SettingsLanguage);

			ReactiveSettingValue.Subscribe(
				x =>
				{
					localizationService.Culture.Value = x.Value switch
					{
						Language.English => CultureInfo.GetCultureInfo("en-US"),
						Language.Turkish => CultureInfo.GetCultureInfo("tr-TR"),
						Language.System => CultureInfo.InstalledUICulture,
						_ => throw new ArgumentOutOfRangeException(nameof(x), x, null)
					};
				});

			this.SaveOnChange();
		}

		protected override Task<object> GetValueBeforeSave(
			LocalizedSettingValue<Language> value,
			CancellationToken cancellationToken)
		{
			return Task.FromResult((object)value.Value);
		}

		protected override Task<LocalizedSettingValue<Language>> GetValueAfterRead(
			object value,
			CancellationToken cancellationToken)
		{
			Language language = (Language)value;

			return Task.FromResult(
				new LocalizedSettingValue<Language>(
					language,
					GetName(_localizableProperties, language)));
		}

		private static IReadOnlyReactiveProperty<string> GetName(
			LocalizableProperties localizableProperties,
			Language language)
		{
			return language switch
			{
				Language.System => localizableProperties.CommonSystem,
				Language.English => localizableProperties.SettingsLanguageEnglish,
				Language.Turkish => localizableProperties.SettingsLanguageTurkish,
				_ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
			};
		}
	}
}