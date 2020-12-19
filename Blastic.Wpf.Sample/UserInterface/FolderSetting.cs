using System.IO;
using Blastic.DynamicControls;
using Blastic.Reactive;
using Blastic.Services.Localization;
using Blastic.Services.Settings;
using Blastic.Settings;
using Blastic.Wpf.Services.Dialog;
using Blastic.Wpf.Settings;

namespace Blastic.Wpf.Sample.UserInterface
{
	public sealed class FolderSetting : FileBrowserSetting
	{
		private readonly ILocalizationService _localizationService;

		public FolderSetting(
			ISettingsStorage settingsStorage,
			IPresenterSource presenterSource,
			ILocalizationService localizationService,
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
			_localizationService = localizationService;
			Element.WithLabel("Workspace folder");
			Element.WithHelp("Workspace folder help content.");

			IsFolderPicker = true;
		}

		public override IReadOnlyReactiveProperty<string> CheckErrorReactive()
		{
			return Directory.Exists(SettingValue)
				? null
				: new LocalizableReactiveProperty(_localizationService, "Blastic.Sample.InvalidWorkspace");
		}
	}
}