using System.Text.Json;
using System.Text.Json.Serialization;
using Blastic.Maui.Sample.Actions;

namespace Blastic.Maui.Sample.Data;

[JsonSerializable(typeof(SetVillage))]
[JsonSerializable(typeof(SetCity))]
[JsonSerializable(typeof(SetRoad))]
public partial class GameActionJsonContext : JsonSerializerContext
{
}

public class GameActionSerializer
{
	private static readonly GameActionJsonContext Context;
	private static readonly Dictionary<string, Type> Types;

	static GameActionSerializer()
	{
		JsonSerializerOptions options = new()
		{
			ReadCommentHandling = JsonCommentHandling.Skip,
			DefaultIgnoreCondition = JsonIgnoreCondition.Never,
			IgnoreReadOnlyFields = false,
			IgnoreReadOnlyProperties = false,
			IncludeFields = false,
			WriteIndented = false,
			AllowTrailingCommas = true,
		};

		Context = new GameActionJsonContext(options);
		Types = new Dictionary<string, Type>();
		
		AddType<SetVillage>();
		AddType<SetCity>();
		AddType<SetRoad>();

		void AddType<T>()
		{
			Type type = typeof(T);
			Types[type.Name] = type;
		}
	}

	public static string Serialize(GameAction action)
	{
		return JsonSerializer.Serialize(action, action.GetType(), Context);
	}

	public static GameAction Deserialize(string typeName, string json)
	{
		if (!Types.TryGetValue(typeName, out Type? type))
		{
			throw new ArgumentOutOfRangeException(
				nameof(typeName),
				$"Can't deserialize unknown type {typeName}");
		}

		return (GameAction)JsonSerializer.Deserialize(json, type, Context)!;
	}
}