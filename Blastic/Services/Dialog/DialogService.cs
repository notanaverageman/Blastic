using System;
using System.Windows;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Blastic.Services.Dialog
{
	public class DialogService : IDialogService
	{
		public bool? ShowDialog<T>(object viewModel) where T : Window
		{
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

			bool? result = options?.Owner == null
				? openFileDialog.ShowDialog()
				: openFileDialog.ShowDialog(options.Owner);

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

			bool? result = options?.Owner == null
				? saveFileDialog.ShowDialog()
				: saveFileDialog.ShowDialog(options.Owner);

			return result == true
				? saveFileDialog.FileName
				: "";
		}

		public string ShowSelectFolderDialog(FileDialogOptions options)
		{
			using CommonFileDialog folderBrowserDialog = new CommonOpenFileDialog
			{
				IsFolderPicker = true,
				InitialDirectory = options?.InitialDirectory ?? ""
			};

			CommonFileDialogResult result = options?.Owner == null
				? folderBrowserDialog.ShowDialog()
				: folderBrowserDialog.ShowDialog(options.Owner);

			return result == CommonFileDialogResult.Ok
				? folderBrowserDialog.FileName
				: "";
		}
	}
}