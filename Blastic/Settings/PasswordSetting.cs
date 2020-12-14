using Blastic.DynamicControls;
using Blastic.DynamicControls.Elements;
using Blastic.Services.Settings;

namespace Blastic.Settings
{
	/// <summary>
	/// A setting that stores a password value. Corresponds to a password box on UI.
	/// </summary>
	public class PasswordSetting : Setting<string>
	{
		/// <summary>
		/// Field to customize the UI behavior.
		/// </summary>
		public PasswordField PasswordField { get; }

		/// <inheritdoc />
		public override IElement Element => PasswordField;

		/// <inheritdoc />
		public PasswordSetting(
			ISettingsStorage settingsStorage,
			IPresenterSource presenterSource,
			string key,
			string defaultValue)
			:
			base(settingsStorage, presenterSource, key, defaultValue)
		{
			PasswordField = new PasswordField(ReactiveSettingValue);
		}
	}
}