using System.IO;
using Blastic.Services.Dialog;
using Blastic.Services.Settings;
using Blastic.UserInterface.Settings;

namespace Blastic.Sample.UserInterface
{
	public sealed class FolderSetting : FileBrowserSetting
	{
		public FolderSetting(ISettingsService settingsService, IDialogService dialogService)
			:
			base(
				settingsService,
				dialogService,
				default,
				"Blastic.Sample.Program.WorkspaceFolder",
				"")
		{
			Element.Label.Value = "Workspace folder";
			Element.Help.Value = "Workspace folder help content.";

			IsFolderPicker = true;
		}

		public override string CheckError()
		{
			return Directory.Exists(SettingValue)
				? ""
				: "Workspace directory does not exist.";
		}
	}
}