using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;

namespace Blastic.Data;

public class Command : IDisposable
{
	private readonly SqliteCommand _command;

	public string CommandText
	{
		get => _command.CommandText;
		set => _command.CommandText = value;
	}

	public Command(SqliteCommand command)
	{
		_command = command;
	}

	public void AddParameterWithValue(string name, object? value)
	{
		SqliteParameter parameter = _command.CreateParameter();
		SetParameter(parameter, value);

		parameter.ParameterName = name;
		_command.Parameters.Add(parameter);
	}

	public void AddParameter(string name)
	{
		AddParameterWithValue(name, null);
	}

	public void SetParameter(string name, object value)
	{
		SqliteParameter parameter = _command.Parameters[name];
		SetParameter(parameter, value);
	}

	public void SetParameter(int index, object value)
	{
		SqliteParameter parameter = _command.Parameters[index];
		SetParameter(parameter, value);
	}

	public void SetParameter(SqliteParameter parameter, object? value)
	{
		value ??= DBNull.Value;

		if (value is DateTime dateTime)
		{
			value = dateTime.ToFileTimeUtc();
		}

		if (value is DateTimeOffset dateTimeOffset)
		{
			value = dateTimeOffset.ToFileTime();
		}

		if (DataReader.IsListOfEnums(value))
		{
			IEnumerable<object> list = ((IList)value).Cast<object>();
			value = string.Join(DataReader.ListSeparator, list.Select(x => (int)x));
		}

		parameter.Value = value;
	}

	public void ClearParameters()
	{
		_command.Parameters.Clear();
	}
	
	public int ExecuteNonQuery()
	{
		return _command.ExecuteNonQuery();
	}

	public T? ExecuteScalar<T>()
	{
		object? result = _command.ExecuteScalar();
		return DataReader.SafeCast<T>(result);
	}

	public DataReader ExecuteReader()
	{
		SqliteDataReader reader = _command.ExecuteReader();
		return new DataReader(reader);
	}

	public void Dispose()
	{
		_command.Dispose();
	}
}