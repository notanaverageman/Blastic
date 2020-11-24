using System;

namespace Blastic.Forms.Sample.Librivox
{
	public class ArchiveOrgResponse
	{
		public int NumFound { get; set; }
		public int Start { get; set; }

		public ArchiveOrgDocument[] Docs { get; set; }

		public ArchiveOrgResponse()
		{
			Docs = Array.Empty<ArchiveOrgDocument>();
		}
	}
}