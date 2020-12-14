using Blastic.DynamicControls;
using Blastic.Forms.Sample.Resources;
using Blastic.Reactive;
using Blastic.Services.Localization;
using Blastic.Services.Settings;

namespace Blastic.Forms.Sample.UserInterface.Settings.Languages
{
	public class LanguageSettingsSection : SettingsSectionViewModel
	{
		public override IReadOnlyReactiveProperty<string> Title { get; }
		public IReadOnlyReactiveProperty<string> Help { get; }

		public LanguageSetting Language { get; }
		
		public LanguageSettingsSection(
			ISettingsStorage settingsStorage,
			IPresenterSource presenterSource,
			ILocalizationService localizationService,
			LocalizableProperties localizableProperties)
			:
			base(settingsStorage)
		{
			Title = localizableProperties.SettingsLanguage;
			Help = localizableProperties.SettingsLanguageHelp;

			Language = new LanguageSetting(
				settingsStorage,
				presenterSource,
				localizationService,
				localizableProperties);
		}
	}
}