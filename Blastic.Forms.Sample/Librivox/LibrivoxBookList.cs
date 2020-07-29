using System;
using System.Collections.Generic;
using Blastic.Forms.Sample.UserInterface;

namespace Blastic.Forms.Sample.Librivox
{
	public class LibrivoxBookList
	{
		private const string ArchiveOrgIdPrefix = "http://www.archive.org/details/";
		private const string ArchiveOrgImageUrlTemplate = "https://archive.org/services/get-item-image.php?identifier=archiveOrgId&mediatype=audio&collection=librivoxaudio";

		public Dictionary<string, LibrivoxBook> books { get; set; }

		public List<BookViewModel> ToViewModels()
		{
			List<BookViewModel> result = new List<BookViewModel>();

			foreach (LibrivoxBook librivoxBook in books.Values)
			{
				if (!int.TryParse(librivoxBook.id, out int bookId))
				{
					Console.WriteLine("Book id is not an int: {0}", librivoxBook.id);
					continue;
				}

				List<AuthorViewModel> authors = new List<AuthorViewModel>();

				foreach (LibrivoxAuthor librivoxAuthor in librivoxBook.authors)
				{
					if (!int.TryParse(librivoxAuthor.id, out int authorId))
					{
						Console.WriteLine("Author id is not an int: {0}", librivoxAuthor.id);
						continue;
					}

					AuthorViewModel author = new AuthorViewModel(authorId);

					author.FirstName.Value = librivoxAuthor.first_name;
					author.LastName.Value = librivoxAuthor.last_name;
					author.DateOfBirth.Value = librivoxAuthor.dob;
					author.DateOfDeath.Value = librivoxAuthor.dod;

					authors.Add(author);
				}

				BookViewModel book = new BookViewModel(bookId, authors);

				string archiveOrgId = librivoxBook.url_iarchive.Replace(ArchiveOrgIdPrefix, "");
				string imageUrl = ArchiveOrgImageUrlTemplate.Replace("archiveOrgId", archiveOrgId);

				book.Title.Value = librivoxBook.title;
				book.Description.Value = librivoxBook.description;
				book.ImageUrl.Value = imageUrl;
				book.Language.Value = librivoxBook.language;
				book.TotalDuration.Value = TimeSpan.FromSeconds(librivoxBook.totaltimesecs);

				result.Add(book);
			}

			return result;
		}
	}
}