using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;

namespace Blastic.Data;

public class DataReader : IDisposable
{
	public const string ListSeparator = ";";

	private readonly SqliteDataReader _dataReader;

	public bool HasRows => _dataReader.HasRows;

	public DataReader(SqliteDataReader dataReader)
	{
		_dataReader = dataReader;
	}

	public bool Read()
	{
		return _dataReader.Read();
	}

	public T? Get<T>(string name)
	{
		return SafeCast<T>(_dataReader[name]);
	}

	public T? Get<T>(int index)
	{
		return SafeCast<T>(_dataReader[index]);
	}

	public List<T>? GetEnumList<T>(string name)
	{
		return SafeCastEnumList<T>(_dataReader[name]);
	}

	internal static T? SafeCast<T>(object? value)
	{
		if (value == null)
		{
			return default;
		}

		if (value == DBNull.Value)
		{
			return default;
		}

		bool isBool = typeof(T) == typeof(bool) || typeof(T) == typeof(bool?);

		if (isBool && (value is int boolInt))
		{
			return (T)(object)Convert.ToBoolean(boolInt);
		}

		if (isBool && (value is long boolLong))
		{
			return (T)(object)Convert.ToBoolean(boolLong);
		}

		bool isInt = typeof(T) == typeof(int) || typeof(T) == typeof(int?);

		if ((isInt || typeof(T).IsEnum) && value is long l)
		{
			return (T)(object)(int)l;
		}

		if (isInt && value is decimal d)
		{
			return (T)(object)(int)d;
		}

		if (typeof(T) == typeof(DateTime) || typeof(T) == typeof(DateTime?))
		{
			return (T)(object)DateTime.FromFileTimeUtc((long)value);
		}

		return (T)value;
	}

	private static List<T>? SafeCastEnumList<T>(object value)
	{
		if (value == DBNull.Value)
		{
			return null;
		}

		if (!IsListOfEnums(typeof(List<T>)))
		{
			throw new ArgumentException("Return type should be a list of enums.");
		}

		string valueAsString = (string)value;
		string[] tokens = valueAsString.Split(new[] { ListSeparator }, StringSplitOptions.RemoveEmptyEntries);

		List<T> result = new();

		foreach (string token in tokens)
		{
			int valueAsInt = int.Parse(token);
			T enumValue = (T)(object)valueAsInt;

			result.Add(enumValue);
		}

		return result;
	}

	public void Dispose()
	{
		_dataReader.Dispose();
	}

	public static bool IsListOfEnums(object value)
	{
		return IsListOfEnums(value.GetType());
	}

	public static bool IsListOfEnums(Type type)
	{
		return
			type.IsGenericType &&
			type.GetGenericTypeDefinition() == typeof(List<>) &&
			type.GetGenericArguments().All(x => x.IsEnum);
	}
}