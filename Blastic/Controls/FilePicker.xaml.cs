using System.Windows;
using Blastic.Services.Dialog;
using Blastic.Services.Dialog.FileFilters;

namespace Blastic.Controls
{
	public partial class FilePicker
	{
		public static readonly DependencyProperty PathProperty = DependencyProperty.Register(
			nameof(PathProperty).Replace("Property", ""),
			typeof(string),
			typeof(FilePicker),
			new FrameworkPropertyMetadata(default, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		public string Path
		{
			get => (string)GetValue(PathProperty);
			set => SetValue(PathProperty, value);
		}

		public static readonly DependencyProperty FilterProperty = DependencyProperty.Register(
			nameof(FilterProperty).Replace("Property", ""),
			typeof(IFileDialogFilter),
			typeof(FilePicker),
			new PropertyMetadata(default));
		public IFileDialogFilter Filter
		{
			get => (IFileDialogFilter)GetValue(FilterProperty);
			set => SetValue(FilterProperty, value);
		}

		public static readonly DependencyProperty DialogServiceProperty = DependencyProperty.Register(
			nameof(DialogServiceProperty).Replace("Property", ""),
			typeof(IDialogService),
			typeof(FilePicker),
			new PropertyMetadata(default));
		public IDialogService DialogService
		{
			get => (IDialogService)GetValue(DialogServiceProperty);
			set => SetValue(DialogServiceProperty, value);
		}

		public static readonly DependencyProperty IsFolderPickerProperty = DependencyProperty.Register(
			nameof(IsFolderPickerProperty).Replace("Property", ""),
			typeof(bool),
			typeof(FilePicker),
			new PropertyMetadata(default));
		public bool IsFolderPicker
		{
			get => (bool)GetValue(IsFolderPickerProperty);
			set => SetValue(IsFolderPickerProperty, value);
		}

		public static readonly DependencyProperty IsSaveFilePickerProperty = DependencyProperty.Register(
			nameof(IsSaveFilePickerProperty).Replace("Property", ""),
			typeof(bool),
			typeof(FilePicker),
			new PropertyMetadata(default));
		public bool IsSaveFilePicker
		{
			get => (bool)GetValue(IsSaveFilePickerProperty);
			set => SetValue(IsSaveFilePickerProperty, value);
		}

		public FilePicker()
		{
			InitializeComponent();
		}

		public void SelectPath()
		{
			void SetIfNotEmpty(string path)
			{
				if (!string.IsNullOrEmpty(path))
				{
					Path = path;
				}
			}

			FileDialogOptions options = new FileDialogOptions(Filter, Window.GetWindow(this), Path);

			if (IsFolderPicker)
			{
				string folderPath = DialogService.ShowSelectFolderDialog(options);
				SetIfNotEmpty(folderPath);

				return;
			}

			string filePath = IsSaveFilePicker
				? DialogService.ShowSaveFileDialog(options)
				: DialogService.ShowOpenFileDialog(options);

			SetIfNotEmpty(filePath);
		}
	}
}