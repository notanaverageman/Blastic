using Blastic.DynamicControls;
using Blastic.Services.Settings;
using Blastic.Settings;

namespace Blastic.Wpf.UserInterface.Logs.Settings
{
	public sealed class OpenWindowOnErrorSetting : BoolSetting
	{
		public OpenWindowOnErrorSetting(
			ISettingsStorage settingsStorage,
			IPresenterSource presenterSource)
			:
			base(settingsStorage, presenterSource, "Log.OpenLogsWindowOnError", false)
		{
			Element.WithLabel("Open logs window on error");
			Element.WithHelp("Open the logs window whenever an error log is printed.");
		}
	}
}