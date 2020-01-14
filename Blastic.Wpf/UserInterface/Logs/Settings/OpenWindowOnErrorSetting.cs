using Blastic.DynamicControls;
using Blastic.Services.Settings;
using Blastic.Settings;

namespace Blastic.Wpf.UserInterface.Logs.Settings
{
	public sealed class OpenWindowOnErrorSetting : BooleanSetting
	{
		public OpenWindowOnErrorSetting(
			ISettingsService settingsService,
			IPresenterSource presenterSource)
			:
			base(settingsService, presenterSource, "Log.OpenLogsWindowOnError", false)
		{
			Element.WithLabel("Open logs window on error");
			Element.WithHelp("Open the logs window whenever an error log is printed.");
		}
	}
}