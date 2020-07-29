using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blastic.Forms.Sample.Data
{
	public class Book
	{
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int Id { get; set; }

		public string Title { get; set; }
		public string Description { get; set; }
		public string Language { get; set; }

		public List<AuthorBookMapping> AuthorBookMappings { get; set; }
		public List<Section> Sections { get; set; }
		public List<Genre> Genres { get; set; }
		public List<Translator> Translators { get; set; }

		public TimeSpan TotalDuration { get; set; }

		public Book()
		{
			AuthorBookMappings = new List<AuthorBookMapping>();
			Sections = new List<Section>();
			Genres = new List<Genre>();
			Translators = new List<Translator>();
		}
	}
}