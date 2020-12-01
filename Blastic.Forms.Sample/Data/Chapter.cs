using System;

namespace Blastic.Forms.Sample.Data
{
	public class Chapter
	{
		public int Id { get; set; }

		public string Title { get; set; }
		public string FileName { get; set; }

		public int Order { get; set; }
		public int SizeInBytes { get; set; }
		public TimeSpan Duration { get; set; }
	}
}