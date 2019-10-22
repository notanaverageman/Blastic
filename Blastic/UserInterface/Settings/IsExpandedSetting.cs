using Blastic.Services.Settings;

namespace Blastic.UserInterface.Settings
{
	public class IsExpandedSetting : BooleanSetting
	{
		public IsExpandedSetting(ISettingsService settingsService, string sectionName)
			:
			base(settingsService, $"Blastic.Settings.IsExpanded.{sectionName}", false)
		{
		}
	}
}