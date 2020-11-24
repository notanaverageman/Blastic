using System.Collections.Generic;
using Blastic.Forms.Sample.UserInterface;

namespace Blastic.Forms.Sample.Librivox
{
	public class ArchiveOrgQueryResult
	{
		private const string ArchiveOrgImageUrlPrefix = "https://archive.org/services/img/";

		public ArchiveOrgResponse Response { get; set; }

		public List<BookViewModel> ToViewModels()
		{
			List<BookViewModel> result = new List<BookViewModel>();

			foreach (ArchiveOrgDocument document in Response.Docs)
			{
				BookViewModel book = new BookViewModel(document.Identifier)
				{
					Title = { Value = document.Title },
					Creator = { Value = document.Creator }
				};

				string imageUrl = ArchiveOrgImageUrlPrefix + document.Identifier;

				book.Title.Value = document.Title;
				book.Description.Value = document.Description;
				book.ImageUrl.Value = imageUrl;

				result.Add(book);
			}

			return result;
		}
	}
}