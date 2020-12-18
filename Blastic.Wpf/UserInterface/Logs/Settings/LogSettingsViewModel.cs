using Blastic.DynamicControls;
using Blastic.Services.Settings;
using Blastic.Wpf.UserInterface.Settings;

namespace Blastic.Wpf.UserInterface.Logs.Settings
{
	public class LogSettingsViewModel : SettingsSectionViewModel
	{
		public override string SectionName => "Logs";
		
		public OpenWindowOnErrorSetting OpenWindowOnErrorSetting { get; }
		
		public LogSettingsViewModel(
			ISettingsStorage settingsStorage,
			IPresenterSource presenterSource)
			:
			base(settingsStorage, presenterSource)
		{
			OpenWindowOnErrorSetting = new OpenWindowOnErrorSetting(settingsStorage, presenterSource);
		}
	}
}