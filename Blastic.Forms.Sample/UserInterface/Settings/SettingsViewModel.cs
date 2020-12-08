using Blastic.Forms.Sample.Icons;
using Blastic.Forms.Sample.Resources;
using Blastic.Forms.Sample.UserInterface.Settings.Languages;
using Blastic.Forms.Sample.UserInterface.Settings.Themes;
using Blastic.Forms.UserInterface;
using Blastic.LifetimeManagement;
using Blastic.Ordering;
using Blastic.Reactive;

namespace Blastic.Forms.Sample.UserInterface.Settings
{
	public class SettingsViewModel : ConductorAllActive<SettingsSectionViewModel>, IShellTab
	{
		public Order Order { get; }
		public IReadOnlyReactiveProperty<string> Title { get; }
		public IReadOnlyReactiveProperty<string> IconGlyph { get; }

		public ThemeSettingsSection ThemeSettings { get; }
		public LanguageSettingsSection LanguageSettings { get; }

		public SettingsViewModel(
			ThemeSettingsSection themeSettings,
			LanguageSettingsSection languageSettings,
			LocalizableProperties localizableProperties)
		{
			ThemeSettings = themeSettings;
			LanguageSettings = languageSettings;

			Order = new Order(3);
			Title = localizableProperties.SettingsTitle;
			IconGlyph = new ReactiveProperty<string>(IconFont.Cog);

			Items.Add(ThemeSettings);
			Items.Add(LanguageSettings);
		}
	}
}