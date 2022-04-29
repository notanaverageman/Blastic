namespace Blastic.Data.Tables;

public abstract class TableBase
{
	protected Connection Connection { get; }

	public TableBase(Connection connection)
	{
		Connection = connection;
	}
}