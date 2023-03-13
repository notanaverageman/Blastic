using Blastic.Data;
using Blastic.Maui.Sample.Data.Tables;
using Opus.Serialization.Migrations;

namespace Blastic.Maui.Sample.Data;

public class GameDatabase : DatabaseBase
{
	public ActionsTable Actions { get; }

	public GameDatabase(GameDatabaseOptions options)
		:
		base(options.ConnectionStringBuilder, "GameMetadata")
	{
		Actions = new ActionsTable(Connection);
		InitializeMigrations();
	}

	private void InitializeMigrations()
	{
		AddMigration(new CreateActionsTable(Connection));
		AddMigration(new CreateLastAppliedActionsTable(Connection));
	}
}