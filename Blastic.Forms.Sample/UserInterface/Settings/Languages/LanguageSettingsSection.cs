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
			ISettingsService settingsService,
			IPresenterSource presenterSource,
			ILocalizationService localizationService,
			LocalizableProperties localizableProperties)
			:
			base(settingsService)
		{
			Title = localizableProperties.SettingsLanguage;
			Help = localizableProperties.SettingsLanguageHelp;

			Language = new LanguageSetting(
				settingsService,
				presenterSource,
				localizationService,
				localizableProperties);
		}
	}
}