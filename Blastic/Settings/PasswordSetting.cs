using Blastic.Controls.DynamicControls.Elements;
using Blastic.Controls.DynamicControls.Elements.Password;
using Blastic.Services.Settings;

namespace Blastic.Settings
{
	public class PasswordSetting : Setting<string>
	{
		public PasswordField PasswordField { get; }
		public override IElement Element => PasswordField;

		public PasswordSetting(
			ISettingsService settingsService,
			string key,
			string defaultValue)
			:
			base(settingsService, key, defaultValue)
		{
			PasswordField = new PasswordField(ReactiveSettingValue);
		}
	}
}