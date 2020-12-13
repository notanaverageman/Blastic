using Blastic.DynamicControls;
using Blastic.DynamicControls.Elements;
using Blastic.Services.Settings;

namespace Blastic.Settings
{
	/// <summary>
	/// A setting that stores a boolean value. Corresponds to a checkbox on UI.
	/// </summary>
	public class BoolSetting : Setting<bool>
	{
		/// <summary>
		/// Field to customize the UI behavior.
		/// </summary>
		public BooleanField BooleanField { get; }

		/// <inheritdoc />
		public override IElement Element => BooleanField;

		/// <inheritdoc />
		public BoolSetting(
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