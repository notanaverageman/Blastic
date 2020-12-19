using Blastic.DynamicControls;
using Blastic.Services.Localization;
using Blastic.Services.Settings;
using Blastic.Wpf.Services.Dialog;
using Blastic.Wpf.UserInterface.Settings;

namespace Blastic.Wpf.Sample.UserInterface
{
	public class TestSettingsViewModel : SettingsSectionViewModel
	{
		public override string SectionName => "Program";

		public FolderSetting FolderSetting { get; }

		public TestSettingsViewModel(
			ISettingsStorage settingsStorage,
			IPresenterSource presenterSource,
			ILocalizationService localizationService,
			IDialogService dialogService)
			:
			base(settingsStorage, presenterSource)
		{
			FolderSetting = new FolderSetting(settingsStorage, presenterSource, localizationService, dialogService);
		}
	}
}