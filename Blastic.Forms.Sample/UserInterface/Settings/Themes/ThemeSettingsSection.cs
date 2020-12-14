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
			ISettingsStorage settingsStorage,
			IPresenterSource presenterSource,
			LocalizableProperties localizableProperties)
			:
			base(settingsStorage)
		{
			Title = localizableProperties.SettingsTheme;
			Help = localizableProperties.SettingsThemeHelp;

			Theme = new ThemeSetting(
				settingsStorage,
				presenterSource,
				localizableProperties);
		}
	}
}