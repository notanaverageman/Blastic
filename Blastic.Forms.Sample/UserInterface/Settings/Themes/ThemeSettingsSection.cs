using Blastic.DynamicControls;
using Blastic.Forms.Sample.Resources;
using Blastic.Reactive;
using Blastic.Services.Settings;

namespace Blastic.Forms.Sample.UserInterface.Settings.Themes
{
	public class ThemeSettingsSection : SettingsSectionViewModel
	{
		public override IReadOnlyReactiveProperty<string> Title { get; }
		public IReadOnlyReactiveProperty<string> Help { get; }

		public ThemeSetting Theme { get; }
		
		public ThemeSettingsSection(
			ISettingsService settingsService,
			IPresenterSource presenterSource,
			LocalizableProperties localizableProperties)
			:
			base(settingsService)
		{
			Title = localizableProperties.SampleSettingsTheme;
			Help = localizableProperties.SampleSettingsThemeHelp;

			Theme = new ThemeSetting(
				settingsService,
				presenterSource,
				localizableProperties);
		}
	}
}