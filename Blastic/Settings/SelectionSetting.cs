using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blastic.DynamicControls;
using Blastic.DynamicControls.Elements;
using Blastic.Reactive;
using Blastic.Services.Settings;

namespace Blastic.Settings
{
	/// <inheritdoc />
	public class SelectionSetting<T> : SelectionSetting<T, T>
	{
		/// <inheritdoc />
		public SelectionSetting(
			ISettingsStorage settingsStorage,
			IPresenterSource presenterSource,
			string key,
			T defaultValue,
			IEnumerable<T> allValues)
			:
			base(settingsStorage, presenterSource, key, defaultValue, allValues)
		{
		}

		/// <inheritdoc />
		protected override Task<T> GetValueAfterRead(T value, CancellationToken cancellationToken)
		{
			return Task.FromResult(value);
		}

		/// <inheritdoc />
		protected override Task<T> GetValueBeforeSave(T value, CancellationToken cancellationToken)
		{
			return Task.FromResult(value);
		}
	}
	
	/// <summary>
	/// A setting that stores a generic value that is selected among multiple choices.
	/// Corresponds to a dropdown on UI.
	/// </summary>
	public abstract class SelectionSetting<T, TStored> : Setting<T, TStored>
	{
		/// <summary>
		/// Field to customize the UI behavior.
		/// </summary>
		public SelectionField<T> SelectionField { get; }

		/// <inheritdoc />
		public override IElement Element => SelectionField;

		/// <summary>
		/// Creates a new instance of <see cref="SelectionSetting{T,TStored}"/>
		/// </summary>
		/// <param name="settingsStorage">The settings storage.</param>
		/// <param name="presenterSource">The presenter source.</param>
		/// <param name="key">Key that is used when reading from or writing to the store.</param>
		/// <param name="defaultValue">Default value to be used when key does not exist in store.</param>
		/// <param name="allValues">The values to choose one from.</param>
		public SelectionSetting(
			ISettingsStorage settingsStorage,
			IPresenterSource presenterSource,
			string key,
			T defaultValue,
			IEnumerable<T> allValues)
			:
			base(settingsStorage, presenterSource, key, defaultValue)
		{
			SelectionField = new SelectionField<T>(ReactiveSettingValue, new ReactiveCollection<T>(allValues));
		}
	}
}