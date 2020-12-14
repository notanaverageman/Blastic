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
		public string Author { get; set; }
		public double Rating { get; set; }

		public TimeSpan TotalDuration { get; set; }

		public List<Chapter> Chapters { get; }

		public Book()
		{
			Chapters = new List<Chapter>();
		}
	}
}