using Blastic.Data;
using Blastic.Data.Migrations;
using Blastic.Ordering;
using Command = Blastic.Data.Command;
using Version = Blastic.Ordering.Version;

namespace Opus.Serialization.Migrations;

public class CreateActionsTable : MigrationBase
{
	public static readonly Version StaticVersion = new(1, 0, 0, 1);

	public override Version Version => StaticVersion;

	public CreateActionsTable(Connection connection) : base(connection)
	{
	}

	public override void MigrateUp()
	{
		using Command command = Connection.CreateCommand();

		command.CommandText = """
			CREATE TABLE Actions (
			    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
			    GameId      TEXT NOT NULL,
			    ActionIndex INTEGER NOT NULL,
			    Type        TEXT NOT NULL,
			    CreatedAt   BIGINT NOT NULL,
			    Data        JSON NOT NULL
			);
			""";

		command.ExecuteNonQuery();
	}

	public override void MigrateDown()
	{
		using Command command = Connection.CreateCommand();

		command.CommandText = "DROP TABLE Actions";
		command.ExecuteNonQuery();
	}
}