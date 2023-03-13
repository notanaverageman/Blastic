using Blastic.Data;
using Blastic.Data.Migrations;
using Blastic.Ordering;
using Command = Blastic.Data.Command;
using Version = Blastic.Ordering.Version;

namespace Opus.Serialization.Migrations;

public class CreateLastAppliedActionsTable : MigrationBase
{
	public static readonly Version StaticVersion = new(1, 0, 0, 2);

	public override Version Version => StaticVersion;

	public CreateLastAppliedActionsTable(Connection connection) : base(connection)
	{
	}

	public override void MigrateUp()
	{
		using Command command = Connection.CreateCommand();

		command.CommandText = """
			CREATE TABLE LastAppliedActions (
			    GameId   TEXT PRIMARY KEY NOT NULL,
			    ActionId INTEGER
			);
			""";

		command.ExecuteNonQuery();
	}

	public override void MigrateDown()
	{
		using Command command = Connection.CreateCommand();

		command.CommandText = "DROP TABLE LastAppliedActions";
		command.ExecuteNonQuery();
	}
}