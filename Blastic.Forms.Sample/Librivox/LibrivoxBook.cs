using System.Collections.Generic;

namespace Blastic.Forms.Sample.Librivox
{
	public class LibrivoxBook
	{
		public string id { get; set; }
		public string title { get; set; }
		public string description { get; set; }
		public string url_text_source { get; set; }
		public string language { get; set; }
		public string copyright_year { get; set; }
		public string num_sections { get; set; }
		public string url_rss { get; set; }
		public string url_zip_file { get; set; }
		public string url_project { get; set; }
		public string url_librivox { get; set; }
		public string url_iarchive { get; set; }
		public string url_other { get; set; }
		public string totaltime { get; set; }
		public int totaltimesecs { get; set; }

		public List<LibrivoxAuthor> authors { get; set; }

		public LibrivoxBook()
		{
			authors = new List<LibrivoxAuthor>();
		}
	}
}