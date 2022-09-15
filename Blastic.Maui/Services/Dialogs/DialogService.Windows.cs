using Blastic.Services.Dialogs;
using System.Windows.Forms;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace Blastic.Maui.Services.Dialogs
{
	public class DialogService : IDialogService
	{
		public bool? ShowDialog<T>(object viewModel)
		{
			return false;
		}

		public string? ShowOpenFileDialog(FileDialogOptions? options)
		{
			OpenFileDialog openFileDialog = new()
			{
				Filter = options?.Filter?.GetFileDialogRepresentation() ?? "",
				Multiselect = options?.IsMultiSelect ?? false,
				InitialDirectory = options?.InitialDirectory ?? ""
			};

			bool? result = openFileDialog.ShowDialog();

			return result == true
				? openFileDialog.FileName
				: null;
		}

		public string? ShowSaveFileDialog(FileDialogOptions? options)
		{
			SaveFileDialog saveFileDialog = new()
			{
				Filter = options?.Filter?.GetFileDialogRepresentation() ?? "",
				InitialDirectory = options?.InitialDirectory ?? "",
				AddExtension = true
			};

			bool? result = saveFileDialog.ShowDialog();

			return result == true
				? saveFileDialog.FileName
				: null;
		}

		public string? ShowSelectFolderDialog(FileDialogOptions? options)
		{
			FolderBrowserDialog dialog = new()
			{
				SelectedPath = options?.InitialDirectory ?? ""
			};

			DialogResult result = dialog.ShowDialog();

			return result == DialogResult.OK
				? dialog.SelectedPath
				: null;
		}
	}
}