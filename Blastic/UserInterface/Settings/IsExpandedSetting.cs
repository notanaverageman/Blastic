using Blastic.DynamicControls;
using Blastic.Services.Settings;
using Blastic.Settings;

namespace Blastic.UserInterface.Settings
{
	public class IsExpandedSetting : BooleanSetting
	{
		public IsExpandedSetting(
			ISettingsService settingsService,
			IPresenterSource presenterSource,
			string sectionName)
			:
			base(settingsService, presenterSource, $"Blastic.Settings.IsExpanded.{sectionName}", false)
		{
		}
	}
}