using System.ComponentModel.DataAnnotations.Schema;

namespace Blastic.Forms.Sample.Data
{
	public class Genre
	{
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int Id { get; set; }
		public string Name { get; set; }
	}
}