using Blastic.Services.Dialog.FileFilters;

namespace Blastic.Services.Dialog
{
	public class FileDialogOptions
	{
		public IFileDialogFilter? Filter { get; }

		public string? InitialDirectory { get; }
		public bool IsMultiSelect { get; }

		public FileDialogOptions(
			IFileDialogFilter? filter = default,
			string? initialDirectory = default,
			bool isMultiSelect = default)
		{
			Filter = filter;
			InitialDirectory = initialDirectory;
			IsMultiSelect = isMultiSelect;
		}
	}
}