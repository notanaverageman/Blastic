using Blastic.DynamicControls;
using Blastic.Services.Settings;
using Blastic.Settings;

namespace Blastic.Wpf.UserInterface.Settings
{
	public class IsExpandedSetting : BoolSetting
	{
		public IsExpandedSetting(
			ISettingsStorage settingsStorage,
			IPresenterSource presenterSource,
			string sectionName)
			:
			base(settingsStorage, presenterSource, $"Blastic.Settings.IsExpanded.{sectionName}", false)
		{
			ShowOnUI.Value = false;
		}
	}
}