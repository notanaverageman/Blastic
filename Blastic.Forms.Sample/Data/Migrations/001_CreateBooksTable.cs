using Blastic.Data;
using Blastic.Data.Migrations;
using Blastic.Ordering;

namespace Blastic.Forms.Sample.Data.Migrations
{
	public class CreateBooksTable : MigrationBase
	{
		public override Version Version { get; } = new(1, 0, 0);

		public CreateBooksTable(Connection connection) : base(connection)
		{
		}

		public override void MigrateUp()
		{
			using Command command = Connection.CreateCommand();
			
			command.CommandText = $@"CREATE TABLE Books (
                                        Id              INTEGER PRIMARY KEY AUTOINCREMENT,
                                        ArchiveOrgId    TEXT,
                                        Title           TEXT,
                                        Description     TEXT
                                    );";

			command.ExecuteNonQuery();
		}

		public override void MigrateDown()
		{
			using Command command = Connection.CreateCommand();

			command.CommandText = "DROP TABLE Books";
			command.ExecuteNonQuery();
		}
	}
}