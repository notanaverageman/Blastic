using Blastic.DynamicControls;
using Blastic.DynamicControls.Elements;
using Blastic.Services.Settings;

namespace Blastic.Settings
{
	/// <inheritdoc />
	public class StringSetting : StringSetting<string>
	{
		/// <inheritdoc />
		public StringSetting(
			ISettingsStorage settingsStorage,
			IPresenterSource presenterSource,
			string key,
			string defaultValue)
			:
			base(settingsStorage, presenterSource, key, defaultValue)
		{
		}

		/// <inheritdoc />
		protected override string GetValueAfterRead(string value)
		{
			return value;
		}

		/// <inheritdoc />
		protected override string GetValueBeforeSave(string value)
		{
			return value;
		}
	}
	
	/// <summary>
	/// A setting that stores a string value. Corresponds to a text box on UI.
	/// </summary>
	public abstract class StringSetting<TStored> : Setting<string, TStored>
	{
		/// <summary>
		/// Field to customize the UI behavior.
		/// </summary>
		public TextField TextField { get; }

		/// <inheritdoc />
		public override IElement Element => TextField;

		/// <inheritdoc />
		public StringSetting(
			ISettingsStorage settingsStorage,
			IPresenterSource presenterSource,
			string key,
			string defaultValue)
			:
			base(settingsStorage, presenterSource, key, defaultValue)
		{
			TextField = new TextField(ReactiveSettingValue);
		}
	}
}