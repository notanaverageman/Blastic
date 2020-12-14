using System.Collections.Generic;
using Blastic.Forms.Sample.Data;

namespace Blastic.Forms.Sample.ArchiveOrg
{
	public class ArchiveOrgQueryResult
	{
		public List<ArchiveOrgDocument> Documents { get; }

		public ArchiveOrgQueryResult()
		{
			Documents = new List<ArchiveOrgDocument>();
		}

		public List<Book> ToBooks()
		{
			List<Book> result = new();

			foreach (ArchiveOrgDocument document in Documents)
			{
				Book book = new()
				{
					ArchiveOrgId = document.Identifier,
					Title = document.Title,
					Author = document.Creator,
					Description = document.Description,
					Rating = document.Rating
				};

				result.Add(book);
			}

			return result;
		}
	}
}