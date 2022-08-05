using Blastic.Services.Dialogs;

namespace Blastic.Maui.Services.Dialogs
{
	public class DialogService : IDialogService
	{
		public bool? ShowDialog<T>(object viewModel)
		{
			return false;
		}

		public string ShowOpenFileDialog(FileDialogOptions? options)
		{
			return "";
		}

		public string ShowSaveFileDialog(FileDialogOptions? options)
		{
			return "";
		}

		public string ShowSelectFolderDialog(FileDialogOptions? options)
		{
			return "";
		}
	}
}