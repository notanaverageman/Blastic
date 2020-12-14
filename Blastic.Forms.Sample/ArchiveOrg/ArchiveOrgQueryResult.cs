using System.Collections.Generic;
using Blastic.Forms.Sample.Data;

namespace Blastic.Forms.Sample.ArchiveOrg
{
	public class ArchiveOrgQueryResult
	{
		public ArchiveOrgResponse Response { get; set; }

		public List<Book> ToBooks()
		{
			List<Book> result = new List<Book>();

			foreach (ArchiveOrgDocument document in Response.Docs)
			{
				Book book = new Book
				{
					ArchiveOrgId = document.Identifier,
					Title = document.Title,
					Author = document.Creator,
					Description = document.Description
				};

				result.Add(book);
			}

			return result;
		}
	}
}