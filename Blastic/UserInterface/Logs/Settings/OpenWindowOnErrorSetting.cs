using Blastic.Services.Settings;
using Blastic.UserInterface.Settings;

namespace Blastic.UserInterface.Logs.Settings
{
	public sealed class OpenWindowOnErrorSetting : BooleanSetting
	{
		public OpenWindowOnErrorSetting(ISettingsService settingsService)
			:
			base(settingsService, "Log.OpenLogsWindowOnError", false)
		{
			Element.Label.Value = "Open logs window on error";
			Element.Help.Value = "Open the logs window whenever an error log is printed.";
		}
	}
}