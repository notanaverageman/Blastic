using System.Collections.Generic;
using Blastic.DynamicControls;
using Blastic.DynamicControls.Elements;
using Blastic.Reactive;
using Blastic.Services.Settings;

namespace Blastic.Settings
{
	/// <summary>
	/// A setting that stores a generic value that is selected among multiple choices.
	/// Corresponds to a dropdown on UI.
	/// </summary>
	public class SelectionSetting<T> : Setting<T>
	{
		/// <summary>
		/// Field to customize the UI behavior.
		/// </summary>
		public SelectionField<T> SelectionField { get; }

		/// <inheritdoc />
		public override IElement Element => SelectionField;

		/// <summary>
		/// Creates a new instance of <see cref="Setting"/>
		/// </summary>
		/// <param name="settingsService">The settings service.</param>
		/// <param name="presenterSource">The presenter source.</param>
		/// <param name="key">Key that is used when reading from or writing to the store.</param>
		/// <param name="defaultValue">Default value to be used when key does not exist in store.</param>
		/// <param name="allValues">The values to choose one from.</param>
		public SelectionSetting(
			ISettingsService settingsService,
			IPresenterSource presenterSource,
			string key,
			T defaultValue,
			IEnumerable<T> allValues)
			:
			base(settingsService, presenterSource, key, defaultValue)
		{
			SelectionField = new SelectionField<T>(ReactiveSettingValue, new ReactiveCollection<T>(allValues));
		}
	}
}