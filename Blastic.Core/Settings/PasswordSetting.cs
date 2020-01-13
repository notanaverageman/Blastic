using Blastic.DynamicControls;
using Blastic.DynamicControls.Elements;
using Blastic.Services.Settings;

namespace Blastic.Settings
{
	public class PasswordSetting : Setting<string>
	{
		public PasswordField PasswordField { get; }
		public override IElement Element => PasswordField;

		public PasswordSetting(
			ISettingsService settingsService,
			IPresenterSource presenterSource,
			string key,
			string defaultValue)
			:
			base(settingsService, presenterSource, key, defaultValue)
		{
			PasswordField = new PasswordField(ReactiveSettingValue);
		}
	}
}