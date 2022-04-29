using Blastic.Ordering;

namespace Blastic.Data.Migrations;

public abstract class MigrationBase
{
	public abstract Version Version { get; }

	protected Connection Connection { get; }

	public MigrationBase(Connection connection)
	{
		Connection = connection;
	}

	public abstract void MigrateUp();
	public abstract void MigrateDown();
}