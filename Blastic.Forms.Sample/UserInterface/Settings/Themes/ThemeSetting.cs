using System;
using System.Threading;
using System.Threading.Tasks;
using Blastic.DynamicControls;
using Blastic.Forms.Sample.Resources;
using Blastic.Reactive;
using Blastic.Services.Settings;
using Blastic.Settings;
using Xamarin.Forms;

namespace Blastic.Forms.Sample.UserInterface.Settings.Themes
{
	public sealed class ThemeSetting : SelectionSetting<LocalizedSettingValue<Theme>, Theme>
	{
		private readonly LocalizableProperties _localizableProperties;

		public ThemeSetting(
			ISettingsStorage settingsStorage,
			IPresenterSource presenterSource,
			LocalizableProperties localizableProperties)
			:
			base(
				settingsStorage,
				presenterSource,
				"Sample.Theme",
				new LocalizedSettingValue<Theme>(Theme.System, GetName(localizableProperties, Theme.System)),
				new LocalizedSettingValue<Theme>[]
				{
					new(Theme.System, GetName(localizableProperties, Theme.System)),
					new(Theme.Light, GetName(localizableProperties, Theme.Light)),
					new(Theme.Dark, GetName(localizableProperties, Theme.Dark)),
				})
		{
			_localizableProperties = localizableProperties;
			
			Element.WithLabel(localizableProperties.Settings.Theme);
			Lifetime.Initialization.Subscribe(Initialize);
		}

		private void Initialize()
		{
			ReactiveSettingValue.Subscribe(
				x =>
				{
					Application.Current.UserAppTheme = x.Value switch
					{
						Theme.Dark => OSAppTheme.Dark,
						Theme.Light => OSAppTheme.Light,
						_ => OSAppTheme.Unspecified
					};
				});

			SaveOnChange = true;
		}

		protected override Task<Theme> GetValueBeforeSave(
			LocalizedSettingValue<Theme> value,
			CancellationToken cancellationToken)
		{
			return Task.FromResult(value.Value);
		}

		protected override Task<LocalizedSettingValue<Theme>> GetValueAfterRead(
			Theme value,
			CancellationToken cancellationToken)
		{
			return Task.FromResult(
				new LocalizedSettingValue<Theme>(
					value,
					GetName(_localizableProperties, value)));
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