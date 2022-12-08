using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Blastic.Services.Dialogs;
using Blastic.ViewManagement;

namespace Blastic.Avalonia.Services.Dialogs;

public class DialogService : IDialogService
{
	private readonly IViewLocator<StyledElement> _viewLocator;

	public DialogService(IViewLocator<StyledElement> viewLocator)
	{
		_viewLocator = viewLocator;
	}

	public async Task<bool?> ShowDialog(object viewModel)
	{
		Window? mainWindow = GetMainWindow();

		if (mainWindow == null)
		{
			return null;
		}

		StyledElement view = _viewLocator.Locate(viewModel);

		if (view is not Window window)
		{
			throw new InvalidOperationException($"{view.GetType()} should inherit from {typeof(Window)}.");
		}

		return await window.ShowDialog<bool?>(mainWindow);
	}

	public async Task<string?> ShowOpenFileDialog(FileDialogOptions? options)
	{
		Window? mainWindow = GetMainWindow();

		if (mainWindow == null)
		{
			return null;
		}

		FilePickerOpenOptions filePickerOpenOptions = new();

		if (options != null)
		{
			filePickerOpenOptions.AllowMultiple = options.IsMultiSelect;

			if (options.Filter != null)
			{
				filePickerOpenOptions.FileTypeFilter = new FilePickerFileType[]
				{
					new(options.Filter.Explanation)
					{
						Patterns = options.Filter.Extensions.Select(x => $"*{x}").ToList()
					}
				};
			}
		}

		IReadOnlyList<IStorageFile> files = await mainWindow.StorageProvider.OpenFilePickerAsync(filePickerOpenOptions);
		string? result = files.FirstOrDefault()?.Name;

		foreach (IStorageFile file in files)
		{
			file.Dispose();
		}


		return result;
	}

	public Task<string?> ShowSaveFileDialog(FileDialogOptions? options)
	{
		return Task.FromResult<string?>(null);
	}

	public Task<string?> ShowSelectFolderDialog(FileDialogOptions? options)
	{
		return Task.FromResult<string?>(null);
	}

	private Window? GetMainWindow()
	{
		if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
		{
			return null;
		}

		return desktop.MainWindow;
	}
}