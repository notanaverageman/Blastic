using System;
using Blastic.DynamicControls;
using Blastic.DynamicControls.Elements;
using Blastic.Forms.Sample.Resources;
using Blastic.Reactive;
using Blastic.Services.Settings;
using Blastic.Settings;
using Xamarin.Forms;

namespace Blastic.Forms.Sample.UserInterface.Settings.Themes
{
	public sealed class ThemeSetting : SelectionSetting<Theme>
	{
		public ThemeSetting(
			ISettingsStorage settingsStorage,
			IPresenterSource presenterSource,
			LocalizableProperties localizableProperties)
			:
			base(
				settingsStorage,
				presenterSource,
				"Sample.Theme",
				Theme.System,
				new SelectionValueWithLabel<Theme>[]
				{
					new(GetName(localizableProperties, Theme.System), Theme.System),
					new(GetName(localizableProperties, Theme.Light), Theme.Light),
					new(GetName(localizableProperties, Theme.Dark), Theme.Dark),
				})
		{
			Element.WithLabel(localizableProperties.Settings.Theme);
			Lifetime.Initialization.Subscribe(Initialize);
		}

		private void Initialize()
		{
			ReactiveSettingValue.Subscribe(
				x =>
				{
					Application.Current.UserAppTheme = x switch
					{
						Theme.Dark => OSAppTheme.Dark,
						Theme.Light => OSAppTheme.Light,
						_ => OSAppTheme.Unspecified
					};
				});

			SaveOnChange = true;
		}
		
		private static IReadOnlyReactiveProperty<string> GetName(
			LocalizableProperties localizableProperties,
			Theme theme)
		{
			return theme switch
			{
				Theme.System => localizableProperties.Common.System,
				Theme.Light => localizableProperties.Settings.Theme.Light,
				Theme.Dark => localizableProperties.Settings.Theme.Dark,
				_ => throw new ArgumentOutOfRangeException(nameof(theme), theme, null)
			};
		}
	}
}