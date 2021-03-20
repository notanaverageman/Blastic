using System.Collections.Generic;
using Blastic.Data;
using Blastic.Data.Migrations;
using Blastic.Forms.Sample.Data.Tables;
using Microsoft.Extensions.Logging;

namespace Blastic.Forms.Sample.Data
{
	public class ProgramDatabase : DatabaseBase
	{
		public BooksTable BooksTable { get; }

		public ProgramDatabase(
			ConnectionFactory connectionFactory,
			ILogger<ProgramDatabase> logger,
			IEnumerable<MigrationBase> migrations)
			:
			base(connectionFactory, logger, migrations)
		{
			BooksTable = new BooksTable(connectionFactory);
		}
	}
}