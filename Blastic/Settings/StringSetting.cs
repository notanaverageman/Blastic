using Blastic.DynamicControls;
using Blastic.DynamicControls.Elements;
using Blastic.Services.Settings;

namespace Blastic.Settings
{
	public class StringSetting : Setting<string>
	{
		public TextField TextField { get; }
		public override IElement Element => TextField;

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