using System.Threading;
using System.Threading.Tasks;
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
		protected override Task<bool> GetValueAfterRead(bool value, CancellationToken cancellationToken)
		{
			return Task.FromResult(value);
		}

		/// <inheritdoc />
		protected override Task<bool> GetValueBeforeSave(bool value, CancellationToken cancellationToken)
		{
			return Task.FromResult(value);
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