using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blastic.Forms.Sample.Data
{
	public class Author
	{
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int Id { get; set; }

		public string FirstName { get; set; }
		public string LastName { get; set; }

		public string DateOfBirth { get; set; }
		public string DateOfDeath { get; set; }

		public List<AuthorBookMapping> AuthorBookMappings { get; set; }

		public Author()
		{
			AuthorBookMappings = new List<AuthorBookMapping>();
		}
	}
}