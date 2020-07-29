namespace Blastic.Forms.Sample.Data
{
	public class AuthorBookMapping
	{
		public int AuthorId { get; set; }
		public int BookId { get; set; }

		public Author Author { get; set; }
		public Book Book { get; set; }
	}
}