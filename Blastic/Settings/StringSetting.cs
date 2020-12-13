using Blastic.DynamicControls;
using Blastic.DynamicControls.Elements;
using Blastic.Services.Settings;

namespace Blastic.Settings
{
	/// <summary>
	/// A setting that stores a string value. Corresponds to a text box on UI.
	/// </summary>
	public class StringSetting : Setting<string>
	{
		/// <summary>
		/// Field to customize the UI behavior.
		/// </summary>
		public TextField TextField { get; }

		/// <inheritdoc />
		public override IElement Element => TextField;

		/// <inheritdoc />
		public StringSetting(
			ISettingsService settingsService,
			IPresenterSource presenterSource,
			string key,
			string defaultValue)
			:
			base(settingsService, presenterSource, key, defaultValue)
		{
			TextField = new TextField(ReactiveSettingValue);
		}
	}
}