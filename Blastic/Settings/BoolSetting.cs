using Blastic.DynamicControls;
using Blastic.DynamicControls.Elements;
using Blastic.Services.Settings;

namespace Blastic.Settings
{
	/// <inheritdoc />
	public class BoolSetting : BoolSetting<bool>
	{
		/// <inheritdoc />
		public BoolSetting(
			ISettingsStorage settingsStorage,
			IPresenterSource presenterSource,
			string key,
			bool defaultValue)
			:
			base(settingsStorage, presenterSource, key, defaultValue)
		{
		}

		/// <inheritdoc />
		protected override bool GetValueAfterRead(bool value)
		{
			return value;
		}

		/// <inheritdoc />
		protected override bool GetValueBeforeSave(bool value)
		{
			return value;
		}
	}
	
	/// <summary>
	/// A setting that stores a boolean value. Corresponds to a checkbox on UI.
	/// </summary>
	public abstract class BoolSetting<TStored> : Setting<bool, TStored>
	{
		/// <summary>
		/// Field to customize the UI behavior.
		/// </summary>
		public BooleanField BooleanField { get; }

		/// <inheritdoc />
		public override IElement Element => BooleanField;

		/// <inheritdoc />
		public BoolSetting(
			ISettingsStorage settingsStorage,
			IPresenterSource presenterSource,
			string key,
			bool defaultValue)
			:
			base(settingsStorage, presenterSource, key, defaultValue)
		{
			BooleanField = new BooleanField(ReactiveSettingValue);
		}
	}
}