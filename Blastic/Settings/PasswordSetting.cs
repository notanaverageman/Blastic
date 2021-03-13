using System.Threading;
using System.Threading.Tasks;
using Blastic.DynamicControls;
using Blastic.DynamicControls.Elements;
using Blastic.Services.Settings;

namespace Blastic.Settings
{
	/// <inheritdoc />
	public class PasswordSetting : PasswordSetting<string>
	{
		/// <inheritdoc />
		public PasswordSetting(
			ISettingsStorage settingsStorage,
			IPresenterSource presenterSource,
			string key,
			string defaultValue)
			:
			base(settingsStorage, presenterSource, key, defaultValue)
		{
		}

		/// <inheritdoc />
		protected override Task<string> GetValueAfterRead(string value, CancellationToken cancellationToken)
		{
			return Task.FromResult(value);
		}

		/// <inheritdoc />
		protected override Task<string> GetValueBeforeSave(string value, CancellationToken cancellationToken)
		{
			return Task.FromResult(value);
		}
	}
	
	/// <summary>
	/// A setting that stores a password value. Corresponds to a password box on UI.
	/// </summary>
	public abstract class PasswordSetting<TStored> : Setting<string, TStored>
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