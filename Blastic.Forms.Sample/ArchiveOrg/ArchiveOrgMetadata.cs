using System.Collections.Generic;

namespace Blastic.Forms.Sample.ArchiveOrg
{
	public class ArchiveOrgMetadata
	{
		public string Description { get; set; }
		public List<ArchiveOrgChapterMetadata> Chapters { get; }

		public ArchiveOrgMetadata()
		{
			Description = "";
			Chapters = new List<ArchiveOrgChapterMetadata>();
		}
	}
}