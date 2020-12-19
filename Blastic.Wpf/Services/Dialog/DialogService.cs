using System;
using System.Windows;
using System.Windows.Forms;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace Blastic.Wpf.Services.Dialog
{
	public class DialogService : IDialogService
	{
		public bool? ShowDialog<T>(object viewModel)
		{
			if (typeof(Window).IsAssignableFrom(typeof(T)))
			{
				throw new InvalidOperationException($"{typeof(T)} should inherit from {typeof(Window)}.");
			}

			Window dialog = (Window) Activator.CreateInstance(typeof(T));
			dialog.DataContext = viewModel;

			return dialog.ShowDialog();
		}

		public string ShowOpenFileDialog(FileDialogOptions options)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Filter = options?.Filter?.GetFileDialogRepresentation() ?? "",
				Multiselect = options?.IsMultiSelect ?? false,
				InitialDirectory = options?.InitialDirectory ?? ""
			};

			bool? result = openFileDialog.ShowDialog();

			return result == true
				? openFileDialog.FileName
				: "";
		}

		public string ShowSaveFileDialog(FileDialogOptions options)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Filter = options?.Filter?.GetFileDialogRepresentation() ?? "",
				InitialDirectory = options?.InitialDirectory ?? "",
				AddExtension = true
			};

			bool? result = saveFileDialog.ShowDialog();

			return result == true
				? saveFileDialog.FileName
				: "";
		}

		public string ShowSelectFolderDialog(FileDialogOptions options)
		{
			FolderBrowserDialog dialog = new FolderBrowserDialog
			{
				SelectedPath = options?.InitialDirectory ?? ""
			};

			DialogResult result = dialog.ShowDialog();

			return result == DialogResult.OK
				? dialog.SelectedPath
				: "";
		}
	}
}