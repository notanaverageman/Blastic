using Blastic.Data.Migrations;
using Blastic.Ordering;

namespace Blastic.Data.Services.Settings
{
	public class CreateSettingsTable : MigrationBase
	{
		public override Version Version { get; }

		public CreateSettingsTable(Connection connection, Version version) : base(connection)
		{
			Version = version;
		}

		public override void MigrateUp()
		{
			using Command command = Connection.CreateCommand();

			command.CommandText = @"
CREATE TABLE Settings (
    Key   TEXT PRIMARY KEY,
    Value TEXT
);";

			command.ExecuteNonQuery();
		}

		public override void MigrateDown()
		{
			using Command command = Connection.CreateCommand();

			command.CommandText = "DROP TABLE Settings";
			command.ExecuteNonQuery();
		}
	}
}