using System.IO;
using Blastic.DynamicControls;
using Blastic.Services.Dialog;
using Blastic.Services.Settings;
using Blastic.Settings;

namespace Blastic.Wpf.Sample.UserInterface
{
	public sealed class FolderSetting : FileBrowserSetting
	{
		public FolderSetting(
			ISettingsStorage settingsStorage,
			IPresenterSource presenterSource,
			IDialogService dialogService)
			:
			base(
				settingsStorage,
				presenterSource,
				dialogService,
				default,
				"Blastic.Sample.Program.WorkspaceFolder",
				"")
		{
			Element.WithLabel("Workspace folder");
			Element.WithHelp("Workspace folder help content.");

			IsFolderPicker = true;
		}

		public override string CheckError()
		{
			return Directory.Exists(SettingValue)
				? null
				: "Workspace directory does not exist.";
		}
	}
}