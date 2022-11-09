using Blastic.Services.Dialogs;

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
			return null;
		}

		public string? ShowSaveFileDialog(FileDialogOptions? options)
		{
			return null;
		}

		public string? ShowSelectFolderDialog(FileDialogOptions? options)
		{
			return null;
		}
	}
}