using Blastic.Data;
using Blastic.Data.Tables;
using Command = Blastic.Data.Command;

namespace Blastic.Maui.Sample.Data.Tables;

public class ActionsTable : TableBase
{
	public ActionsTable(Connection connection) : base(connection)
	{
	}
	
	public int? GetLastAppliedActionId(string gameId)
	{
		using Command command = Connection.CreateCommand();
		
		command.CommandText = """
			SELECT ActionId FROM LastAppliedActions
			WHERE GameId=@GameId
			""";

		command.AddParameterWithValue("@GameId", gameId);
		
		return command.ExecuteScalar<int?>();
	}

	public GameAction Get(int actionId)
	{
		using Command command = Connection.CreateCommand();
		
		command.CommandText = """
			SELECT * FROM Actions
			WHERE Id=@ActionId
			""";

		command.AddParameterWithValue("@ActionId", actionId);
		using DataReader reader = command.ExecuteReader();

		if (!reader.Read())
		{
			throw new EntryNotFoundException($"Action with id '{actionId}' not found.");
		}

		GameAction action = ReadAction(reader);

		return action;
	}

	public List<GameAction> GetActions(string gameId, int count)
	{
		List<GameAction> actions = new();

		using Command command = Connection.CreateCommand();
		
		command.CommandText = """
			SELECT * FROM Actions
			WHERE GameId=@GameId
			ORDER BY ActionIndex ASC
			LIMIT @Limit
			""";

		command.AddParameterWithValue("@GameId", gameId);
		command.AddParameterWithValue("@Limit", count);

		using DataReader reader = command.ExecuteReader();

		while (reader.Read())
		{
			GameAction action = ReadAction(reader);
			actions.Add(action);
		}
		
		return actions;
	}
	
	// Returned actions are in ascending order.
	public List<GameAction> GetActionsBeforeId(string gameId, int actionId, int count)
	{
		// Use DESC to load the newest actions first.
		List<GameAction> actions = GetActions(gameId, actionId, count, "<", "DESC");
		
		// Revert the list to return the actions in ascending order.
		actions.Reverse();

		return actions;
	}
	
	// Returned actions are in descending order.
	public List<GameAction> GetActionsAfterId(string gameId, int actionId, int count)
	{
		// Use ASC to load the oldest actions first.
		List<GameAction> actions = GetActions(gameId, actionId, count, ">", "ASC");

		// Revert the list to return the actions in descending order.
		actions.Reverse();

		return actions;
	}

	private List<GameAction> GetActions(
		string gameId,
		int actionId,
		int count,
		string comparisonOperator,
		string orderOperator)
	{
		List<GameAction> actions = new();

		using Command command = Connection.CreateCommand();
		
		command.CommandText = $"""
			SELECT * FROM Actions
			WHERE GameId=@GameId AND ActionIndex{comparisonOperator}(
					SELECT ActionIndex FROM Actions
					WHERE Id=@ActionId
				)
			ORDER BY ActionIndex {orderOperator}
			LIMIT @Limit
			""";
		
		command.AddParameterWithValue("@GameId", gameId);
		command.AddParameterWithValue("@ActionId", actionId);
		command.AddParameterWithValue("@Limit", count);

		using DataReader reader = command.ExecuteReader();

		while (reader.Read())
		{
			GameAction action = ReadAction(reader);
			actions.Add(action);
		}

		return actions;
	}

	public void Create(GameAction action, string gameId)
	{
		using Command command = Connection.CreateCommand();

		command.CommandText = """
			INSERT INTO Actions (GameId, Type, CreatedAt, Data, ActionIndex)
			VALUES (
				@GameId,
				@Type,
				@CreatedAt,
				@Data,
				IFNULL((SELECT MAX(ActionIndex) FROM Actions WHERE GameId=@GameId), -1) + 1);
			SELECT last_insert_rowid();
			""";

		command.AddParameterWithValue("@GameId", gameId);
		command.AddParameterWithValue("@Type", action.GetType().Name);
		command.AddParameterWithValue("@CreatedAt", DateTime.UtcNow);
		command.AddParameterWithValue("@Data", GameActionSerializer.Serialize(action));

		int id = command.ExecuteScalar<int>();

		action.Id = id;
	}

	public void DeleteAfter(GameAction? action, string gameId)
	{
		using Command command = Connection.CreateCommand();

		command.CommandText = """
			DELETE FROM Actions 
			WHERE GameId=@GameId
			""";

		if (action != null)
		{
			command.CommandText += @" AND ActionIndex>(SELECT ActionIndex FROM Actions WHERE Id=@Id);";
			command.AddParameterWithValue("@Id", action.Id);
		}

		command.AddParameterWithValue("@GameId", gameId);
		command.ExecuteNonQuery();
	}

	public void UpdateLastAppliedAction(GameAction? action, string gameId)
	{
		using Command command = Connection.CreateCommand();
		
		command.CommandText = """
			INSERT OR REPLACE INTO LastAppliedActions (GameId, ActionId)
			VALUES (@GameId, @ActionId);
			""";

		command.AddParameterWithValue("@GameId", gameId);
		command.AddParameterWithValue("@ActionId", action?.Id);

		command.ExecuteNonQuery();
	}

	private GameAction ReadAction(DataReader reader)
	{
		int id = reader.Get<int>("Id");
		string type = reader.Get<string>("Type")!;
		string data = reader.Get<string>("Data")!;

		GameAction action = GameActionSerializer.Deserialize(data, type);
		action.Id = id;

		return action;
	}
}