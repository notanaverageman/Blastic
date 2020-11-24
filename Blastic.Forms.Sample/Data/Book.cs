using System;
using System.Collections.Generic;

namespace Blastic.Forms.Sample.Data
{
	public class Book
	{
		public int Id { get; set; }
		public string ArchiveOrgId { get; set; }

		public string Title { get; set; }
		public string Description { get; set; }
		public string Language { get; set; }

		public List<Author> Author { get; set; }
		public List<Section> Sections { get; set; }
		public List<Genre> Genres { get; set; }
		public List<Translator> Translators { get; set; }

		public TimeSpan TotalDuration { get; set; }

		public Book()
		{
			Author = new List<Author>();
			Sections = new List<Section>();
			Genres = new List<Genre>();
			Translators = new List<Translator>();
		}
	}
}