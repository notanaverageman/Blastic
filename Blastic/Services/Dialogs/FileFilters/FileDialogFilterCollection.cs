using System.Collections.Generic;
using System.Linq;

namespace Blastic.Services.Dialogs.FileFilters;

public class FileDialogFilterCollection : List<FileDialogFilter>, IFileDialogFilter
{
	public string Explanation => string.Join(", ", this.Select(x => x.Explanation));
	public IEnumerable<string> Extensions => this.SelectMany(filter => filter.Extensions);

	public string GetFileDialogRepresentation()
	{
		return string.Join("|", this.Select(filter => filter.GetFileDialogRepresentation()));
	}
}