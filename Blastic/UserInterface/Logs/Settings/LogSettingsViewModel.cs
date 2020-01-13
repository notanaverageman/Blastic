using Blastic.DynamicControls;
using Blastic.Services.Settings;
using Blastic.UserInterface.Settings;

namespace Blastic.UserInterface.Logs.Settings
{
	public class LogSettingsViewModel : SettingsSectionViewModel
	{
		public override string SectionName => "Logs";
		
		public OpenWindowOnErrorSetting OpenWindowOnErrorSetting { get; }
		
		public LogSettingsViewModel(
			ISettingsService settingsService,
			IPresenterSource presenterSource)
			:
			base(settingsService, presenterSource)
		{
			OpenWindowOnErrorSetting = new OpenWindowOnErrorSetting(settingsService, presenterSource);

			RegisterForUI(OpenWindowOnErrorSetting);
		}
	}
}