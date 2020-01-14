using Blastic.Commanding;
using Blastic.DynamicControls;
using Blastic.DynamicControls.Elements;
using Blastic.DynamicControls.Properties;
using Blastic.Services.Dialog;
using Blastic.Services.Dialog.FileFilters;
using Blastic.Services.Settings;

namespace Blastic.Settings
{
	public class FileBrowserSetting : Setting<string>
	{
		private readonly IDialogService _dialogService;
		private readonly IFileDialogFilter _filter;

		public GroupElement GroupField { get; }
		public override IElement Element => GroupField;

		public bool IsFolderPicker { get; set; }
		public bool IsSaveFilePicker { get; set; }

		public Command BrowseCommand { get; }
		
		public FileBrowserSetting(
			ISettingsService settingsService,
			IPresenterSource presenterSource,
			IDialogService dialogService,
			IFileDialogFilter filter,
			string key,
			string defaultValue)
			:
			base(settingsService, presenterSource, key, defaultValue)
		{
			_dialogService = dialogService;
			_filter = filter;

			BrowseCommand = new Command().WithSubscribe(x =>
			{
				Browse();
			});

			GroupField = new GroupElement();
			
			// Configure after creating to be able to use internal objects, e.g. Label.
			GroupField
				.AddText(ReactiveSettingValue, x => x
					.WithColumnWidth(new GridLength(1, GridUnitType.Star))
					.WithLabel(GroupField.Label))
				.AddAction(BrowseCommand, x => x
					.WithLabel("Browse"));
			// TODO: Icon
		}

		public void Browse()
		{
			void SetIfNotEmpty(string path)
			{
				if (!string.IsNullOrEmpty(path))
				{
					ReactiveSettingValue.Value = path;
				}
			}

			FileDialogOptions options = new FileDialogOptions(_filter, initialDirectory: SettingValue);

			if (IsFolderPicker)
			{
				string folderPath = _dialogService.ShowSelectFolderDialog(options);
				SetIfNotEmpty(folderPath);

				return;
			}

			string filePath = IsSaveFilePicker
				? _dialogService.ShowSaveFileDialog(options)
				: _dialogService.ShowOpenFileDialog(options);

			SetIfNotEmpty(filePath);
		}
	}
}