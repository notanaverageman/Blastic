using Blastic.Services.Settings;
using Blastic.Settings;

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