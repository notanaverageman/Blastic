using System.Collections.Generic;

namespace Blastic.Forms.Sample.ArchiveOrg
{
	public class ArchiveOrgDocument
	{
		public string Identifier { get; set; }

		public string Title { get; set; }
		public string Creator { get; set; }
		public string Description { get; set; }

		public double Rating { get; set; }
		public int Downloads { get; set; }

		public List<string> Tags { get; }

		public ArchiveOrgDocument()
		{
			Tags = new List<string>();
		}
	}
}