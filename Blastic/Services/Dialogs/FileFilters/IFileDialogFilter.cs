using System.Collections.Generic;

namespace Blastic.Services.Dialogs.FileFilters;

public interface IFileDialogFilter
{
	IEnumerable<string> Extensions { get; }
	string GetFileDialogRepresentation();
}