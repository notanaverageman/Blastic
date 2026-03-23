using Blastic.Services.Dialogs;
using System.Threading.Tasks;

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
			return Task.FromResult<string?>(null);
		}

		public Task<string?> ShowSaveFileDialog(FileDialogOptions? options)
		{
			return Task.FromResult<string?>(null);
		}

		public Task<string?> ShowSelectFolderDialog(FileDialogOptions? options)
		{
			return Task.FromResult<string?>(null);
		}
	}
}