using Blastic.DynamicControls;
using Blastic.Services.Dialog;
using Blastic.Services.Settings;
using Blastic.UserInterface.Settings;

namespace Blastic.Sample.UserInterface
{
	public class TestSettingsViewModel : SettingsSectionViewModel
	{
		public override string SectionName => "Program";

		public FolderSetting FolderSetting { get; }

		public TestSettingsViewModel(
			ISettingsService settingsService,
			IPresenterSource presenterSource,
			IDialogService dialogService)
			:
			base(settingsService, presenterSource)
		{
			FolderSetting = new FolderSetting(settingsService, presenterSource, dialogService);
		}
	}
}