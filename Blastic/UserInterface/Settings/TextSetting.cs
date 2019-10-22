using Blastic.Controls.DynamicControls.Elements;
using Blastic.Controls.DynamicControls.Elements.Text;
using Blastic.Services.Settings;

namespace Blastic.UserInterface.Settings
{
	public class TextSetting : Setting<string>
	{
		public TextField TextField { get; }
		public override IElement Element => TextField;

		public TextSetting(
			ISettingsService settingsService,
			string key,
			string defaultValue)
			:
			base(settingsService, key, defaultValue)
		{
			TextField = new TextField(ReactiveSettingValue);
		}
	}
}