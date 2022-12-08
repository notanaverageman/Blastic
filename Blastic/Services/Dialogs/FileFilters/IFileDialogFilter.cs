using System.Collections.Generic;

namespace Blastic.Services.Dialogs.FileFilters;

public interface IFileDialogFilter
{
	string Explanation { get; }
	IEnumerable<string> Extensions { get; }
	string GetFileDialogRepresentation();
}