using Blastic.DynamicControls;
using Blastic.DynamicControls.Elements;
using Blastic.Services.Settings;

namespace Blastic.Settings
{
	public class BooleanSetting : Setting<bool>
	{
		public BooleanField BooleanField { get; }
		public override IElement Element => BooleanField;

		public BooleanSetting(
			ISettingsService settingsService,
			IPresenterSource presenterSource,
			string key,
			bool defaultValue)
			:
			base(settingsService, presenterSource, key, defaultValue)
		{
			BooleanField = new BooleanField(ReactiveSettingValue);
		}
	}
}