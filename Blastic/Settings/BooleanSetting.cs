using Blastic.Controls.DynamicControls.Elements;
using Blastic.Controls.DynamicControls.Elements.Boolean;
using Blastic.Services.Settings;

namespace Blastic.Settings
{
	public class BooleanSetting : Setting<bool>
	{
		public BooleanField BooleanField { get; }
		public override IElement Element => BooleanField;

		public BooleanSetting(
			ISettingsService settingsService,
			string key,
			bool defaultValue)
			:
			base(settingsService, key, defaultValue)
		{
			BooleanField = new BooleanField(ReactiveSettingValue);
		}
	}
}