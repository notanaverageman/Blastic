using Blastic.Execution;
using Blastic.Services.Settings;
using Blastic.UserInterface.Settings;

namespace Blastic.UserInterface.Logs.Settings
{
	public class LogSettingsViewModel : SettingsSectionViewModel
	{
		public override string SectionName => "Logs";
		
		public OpenWindowOnErrorSetting OpenWindowOnErrorSetting { get; }
		
		public LogSettingsViewModel(
			ExecutionContextFactory executionContextFactory,
			ISettingsService settingsService)
			:
			base(executionContextFactory, settingsService)
		{
			OpenWindowOnErrorSetting = new OpenWindowOnErrorSetting(settingsService);

			RegisterForUI(OpenWindowOnErrorSetting);
		}
	}
}