using System.Collections.Generic;
using Blastic.DynamicControls;
using Blastic.DynamicControls.Elements;
using Blastic.Reactive;
using Blastic.Services.Settings;

namespace Blastic.Settings
{
	public class SelectionSetting<T> : Setting<T>
	{
		public SelectionField<T> SelectionField { get; }
		public override IElement Element => SelectionField;

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