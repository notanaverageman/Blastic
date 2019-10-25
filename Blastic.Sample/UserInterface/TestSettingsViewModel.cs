using Blastic.Execution;
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
			ExecutionContextFactory executionContextFactory,
			ISettingsService settingsService,
			IDialogService dialogService)
			:
			base(executionContextFactory, settingsService)
		{
			FolderSetting = new FolderSetting(settingsService, dialogService);
			RegisterForUI(FolderSetting);
		}
	}
}