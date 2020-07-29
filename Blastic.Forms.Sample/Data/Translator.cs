using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blastic.Forms.Sample.Data
{
	public class Translator
	{
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int Id { get; set; }

		public string FirstName { get; set; }
		public string LastName { get; set; }

		public string DateOfBirth { get; set; }
		public string DateOfDeath { get; set; }
	}
}