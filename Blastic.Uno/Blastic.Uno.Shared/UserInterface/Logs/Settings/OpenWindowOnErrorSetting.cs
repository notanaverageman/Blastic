using Blastic.Controls.DynamicControls.Elements;
using Blastic.Services.Settings;
using Blastic.Settings;

namespace Blastic.UserInterface.Logs.Settings
{
	public sealed class OpenWindowOnErrorSetting : BooleanSetting
	{
		public OpenWindowOnErrorSetting(ISettingsService settingsService)
			:
			base(settingsService, "Log.OpenLogsWindowOnError", false)
		{
			Element.WithLabel("Open logs window on error");
			Element.WithHelp("Open the logs window whenever an error log is printed.");
		}
	}
}