using System.Threading.Tasks;
using Blastic.Services.Dialogs;
using System.Windows.Forms;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace Blastic.Maui.Services.Dialogs
{
	public class DialogService : IDialogService
	{
		public Task<bool?> ShowDialog(object viewModel)
		{
			return Task.FromResult<bool?>(null);
		}

		public Task<string?> ShowOpenFileDialog(FileDialogOptions? options)
		{
			OpenFileDialog openFileDialog = new()
			{
				Filter = options?.Filter?.GetFileDialogRepresentation() ?? "",
				Multiselect = options?.IsMultiSelect ?? false,
				InitialDirectory = options?.InitialDirectory ?? ""
			};

			bool? result = openFileDialog.ShowDialog();

			string? fileName = result == true
				? openFileDialog.FileName
				: null;

			return Task.FromResult(fileName);
		}

		public Task<string?> ShowSaveFileDialog(FileDialogOptions? options)
		{
			SaveFileDialog saveFileDialog = new()
			{
				Filter = options?.Filter?.GetFileDialogRepresentation() ?? "",
				InitialDirectory = options?.InitialDirectory ?? "",
				AddExtension = true
			};

			bool? result = saveFileDialog.ShowDialog();

			string? fileName = result == true
				? saveFileDialog.FileName
				: null;

			return Task.FromResult(fileName);
		}

		public Task<string?> ShowSelectFolderDialog(FileDialogOptions? options)
		{
			FolderBrowserDialog dialog = new()
			{
				SelectedPath = options?.InitialDirectory ?? ""
			};

			DialogResult result = dialog.ShowDialog();

			string? folderPath = result == DialogResult.OK
				? dialog.SelectedPath
				: null;

			return Task.FromResult(folderPath);
		}
	}
}