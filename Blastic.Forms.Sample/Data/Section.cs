using System;
using System.Collections.Generic;

namespace Blastic.Forms.Sample.Data
{
	public class Section
	{
		public int Id { get; set; }

		public int SectionNumber { get; set; }
		public string Title { get; set; }

		public string Language { get; set; }
		public TimeSpan Duration { get; set; }

		public string Url { get; set; }

		public List<Reader> Readers { get; set; }

		public Section()
		{
			Readers = new List<Reader>();
		}
	}
}