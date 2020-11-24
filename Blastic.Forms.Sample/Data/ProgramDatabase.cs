using System.Collections.Generic;
using Blastic.Data;
using Blastic.Forms.Data.ProgramData.Migrations;
using Blastic.Forms.Sample.Data.Tables;
using Microsoft.Extensions.Logging;

namespace Blastic.Forms.Sample.Data
{
	public class ProgramDatabase : Forms.Data.ProgramData.ProgramDatabase
	{
		public BooksTable BooksTable { get; }

		public ProgramDatabase(
			ConnectionFactory connectionFactory,
			ILogger<ProgramDatabase> logger,
			IEnumerable<ProgramDatabaseMigrationBase> migrations)
			:
			base(connectionFactory, logger, migrations)
		{
			BooksTable = new BooksTable(connectionFactory);
		}
	}
}