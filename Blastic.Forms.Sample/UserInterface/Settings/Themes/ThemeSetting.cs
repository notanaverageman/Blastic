using System;
using Blastic.DynamicControls;
using Blastic.Forms.Sample.Resources;
using Blastic.Services.Settings;
using Blastic.Settings;
using Xamarin.Forms;

namespace Blastic.Forms.Sample.UserInterface.Settings.Themes
{
	public sealed class ThemeSetting : SelectionSetting<Theme>
	{
		public ThemeSetting(
			ISettingsService settingsService,
			IPresenterSource presenterSource,
			LocalizableProperties localizableProperties)
			:
			base(
				settingsService,
				presenterSource,
				"Sample.Theme",
				Theme.System,
				new []
				{
					Theme.System,
					Theme.Light,
					Theme.Dark
				})
		{
			Element.WithLabel(localizableProperties.SampleSettingsTheme);

			Lifetime.Initialization.Subscribe(Initialize);
		}

		private void Initialize()
		{
			ReactiveSettingValue.Subscribe(
				x =>
				{
					Application.Current.UserAppTheme = x switch
					{
						Theme.System => OSAppTheme.Unspecified,
						Theme.Light => OSAppTheme.Light,
						_ => OSAppTheme.Dark
					};
				});
		}
	}
}