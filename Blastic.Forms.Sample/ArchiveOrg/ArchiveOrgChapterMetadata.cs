using System;

namespace Blastic.Forms.Sample.ArchiveOrg
{
	public class ArchiveOrgChapterMetadata
	{
		public int Track { get; set; }
		public string Title { get; set; }
		public string FileName { get; set; }
		public int SizeInBytes { get; set; }
		public TimeSpan Duration { get; set; }
		public string Sha1 { get; set; }

		public ArchiveOrgChapterMetadata()
		{
			Track = 0;
			Title = "";
			FileName = "";
			SizeInBytes = 0;
			Duration = TimeSpan.Zero;
			Sha1 = "";
		}
	}
}