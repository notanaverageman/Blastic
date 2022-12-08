using Blastic.Data;
using Blastic.Forms.Sample.Data.Migrations;
using Blastic.Forms.Sample.Data.Tables;
using Microsoft.Data.Sqlite;

namespace Blastic.Forms.Sample.Data
{
	public class ProgramDatabase : DatabaseBase
	{
		public BooksTable BooksTable { get; }

		public ProgramDatabase(SqliteConnectionStringBuilder connectionStringBuilder)
			:
			base(connectionStringBuilder, "ProgramMetadata")
		{
			BooksTable = new BooksTable(Connection);

			AddMigration(new CreateBooksTable(Connection));
		}
	}
}