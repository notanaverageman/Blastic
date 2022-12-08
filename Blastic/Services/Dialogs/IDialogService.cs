using System.Threading.Tasks;

namespace Blastic.Services.Dialogs
{
	public interface IDialogService
	{
		Task<bool?> ShowDialog(object viewModel);

		Task<string?> ShowOpenFileDialog(FileDialogOptions? options = default);
		Task<string?> ShowSaveFileDialog(FileDialogOptions? options = default);

		Task<string?> ShowSelectFolderDialog(FileDialogOptions? options = default);
	}
}