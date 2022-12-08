using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Blastic.Services.Dialogs;
using Blastic.ViewManagement;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace Blastic.Wpf.Services.Dialogs
{
	public class DialogService : IDialogService
	{
		private readonly IViewLocator<FrameworkElement> _viewLocator;

		public DialogService(IViewLocator<FrameworkElement> viewLocator)
		{
			_viewLocator = viewLocator;
		}

		public Task<bool?> ShowDialog(object viewModel)
		{
			FrameworkElement view = _viewLocator.Locate(viewModel);

			if (view is not Window window)
			{
				throw new InvalidOperationException($"{view.GetType()} should inherit from {typeof(Window)}.");
			}
			
			window.DataContext = viewModel;
			bool? result = window.ShowDialog();

			return Task.FromResult(result);
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