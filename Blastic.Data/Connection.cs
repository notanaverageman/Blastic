using System;
using Microsoft.Data.Sqlite;

namespace Blastic.Data;

public class Connection : IDisposable
{
	private readonly SqliteConnection _dbConnection;
	private SqliteTransaction? _dbTransaction;

	private int _nestedTransactionCount;

	public bool HasTransaction => _dbTransaction != null;

	public Connection(SqliteConnection dbConnection)
	{
		_dbConnection = dbConnection;
	}

	public void Open()
	{
		_dbConnection.Open();
	}

	public void BeginTransaction()
	{
		if (_dbTransaction != null)
		{
			_nestedTransactionCount++;
			return;
		}

		_dbTransaction = _dbConnection.BeginTransaction();
	}

	public void CommitTransaction()
	{
		if (_nestedTransactionCount > 0)
		{
			_nestedTransactionCount--;
			return;
		}

		_dbTransaction!.Commit();
		_dbTransaction = null;
	}

	public void RollbackTransaction()
	{
		if (_nestedTransactionCount > 0)
		{
			_nestedTransactionCount--;
			return;
		}

		_dbTransaction!.Rollback();
		_dbTransaction = null;
	}
	
	public Command CreateCommand()
	{
		SqliteCommand command = _dbConnection.CreateCommand();

		if (_dbTransaction != null)
		{
			command.Transaction = _dbTransaction;
		}

		return new Command(command);
	}

	public void Dispose()
	{
		_dbTransaction?.Dispose();
		_dbConnection.Dispose();
	}
}