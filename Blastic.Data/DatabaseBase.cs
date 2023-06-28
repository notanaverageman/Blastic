using System;
using System.Collections.Generic;
using System.Linq;
using Blastic.Data.Migrations;
using Blastic.Data.Tables;
using Microsoft.Data.Sqlite;
using Version = Blastic.Ordering.Version;

namespace Blastic.Data;

public abstract class DatabaseBase : DatabaseBase<MetadataTable>
{
	protected DatabaseBase(
		SqliteConnectionStringBuilder connectionStringBuilder,
		string metadataTableName)
		:
		base(connectionStringBuilder, x => new MetadataTable(x, metadataTableName))
	{
	}
}

public abstract class DatabaseBase<T> : IDisposable where T : MetadataTable
{
	private readonly SortedSet<MigrationBase> _migrations;
	private readonly SqliteConnection _sqliteConnection;

	protected readonly Connection Connection;

	public T Metadata { get; }

	public bool HasTransaction => Connection.HasTransaction;
	public bool IsInMemory => string.IsNullOrEmpty(_sqliteConnection.DataSource);
	public string DataSource => _sqliteConnection.DataSource;

	public DatabaseBase(
		SqliteConnectionStringBuilder connectionStringBuilder,
		Func<Connection, T> metadataTableFactory)
	{
		SetConnectionStringDefaults(connectionStringBuilder);

		_sqliteConnection = new SqliteConnection(connectionStringBuilder.ConnectionString);

		Connection = new Connection(_sqliteConnection);
		Metadata = metadataTableFactory(Connection);

		_migrations = new SortedSet<MigrationBase>(MigrationComparer.Instance)
		{
			new CreateMetadataTable(Connection, Metadata.TableName)
		};
	}

	private void SetConnectionStringDefaults(SqliteConnectionStringBuilder connectionStringBuilder)
	{
		connectionStringBuilder.Pooling = false;
		connectionStringBuilder.ForeignKeys = true;
	}

	public void OpenConnection()
	{
		Connection.Open();
	}
	
	public void BeginTransaction()
	{
		Connection.BeginTransaction();
	}
	
	public void CommitTransaction()
	{
		Connection.CommitTransaction();
	}
	
	public void RollbackTransaction()
	{
		Connection.RollbackTransaction();
	}

	public void Clone(SqliteConnectionStringBuilder connectionStringBuilder)
	{
		SetConnectionStringDefaults(connectionStringBuilder);
		
		using SqliteConnection sqliteConnection = new(connectionStringBuilder.ConnectionString);
		sqliteConnection.Open();

		_sqliteConnection.BackupDatabase(sqliteConnection);
	}

	public void MoveTo(SqliteConnectionStringBuilder connectionStringBuilder)
	{
		Clone(connectionStringBuilder);

		_sqliteConnection.Close();
		_sqliteConnection.ConnectionString = connectionStringBuilder.ConnectionString;
		_sqliteConnection.Open();
	}

	public void SetPageSize(int bytes)
	{
		using Command command = Connection.CreateCommand();
		command.CommandText = $"PRAGMA page_size = {bytes}";

		command.ExecuteNonQuery();
	}
	
	public bool IsMigrationAvailable()
	{
		Version? currentVersion = Metadata.GetVersion();
		Version newVersion = _migrations.Max(x => x.Version);

		return currentVersion != newVersion;
	}

	public void Migrate(Version? targetVersion = null)
	{
		Connection.BeginTransaction();

		try
		{
			Version? currentVersion = Metadata.GetVersion();
			Version newVersion = Migrate(currentVersion, targetVersion);

			Metadata.SetVersion(newVersion);

			Connection.CommitTransaction();
		}
		catch (Exception)
		{
			Connection.RollbackTransaction();
			throw;
		}
		
	}

	private Version Migrate(Version? currentVersion, Version? targetVersion)
	{
		targetVersion ??= _migrations.Last().Version;
		
		if (currentVersion == targetVersion)
		{
			return targetVersion;
		}
		
		if (currentVersion == null || currentVersion < targetVersion)
		{
			foreach (MigrationBase migration in _migrations)
			{
				if (migration.Version <= currentVersion || migration.Version > targetVersion)
				{
					continue;
				}

				migration.MigrateUp();
			}
		}
		else
		{
			// Migrate down in reverse order.
			foreach (MigrationBase migration in _migrations.Reverse())
			{
				if (migration.Version <= currentVersion || migration.Version > targetVersion)
				{
					continue;
				}

				migration.MigrateDown();
			}
		}
		
		return targetVersion;
	}

	protected void AddMigration(MigrationBase migration)
	{
		_migrations.Add(migration);
	}

	public void Dispose()
	{
		Connection.Dispose();
	}

	private class MigrationComparer : IComparer<MigrationBase>
	{
		public static readonly MigrationComparer Instance = new();

		public int Compare(MigrationBase? x, MigrationBase? y)
		{
			if (ReferenceEquals(x, y))
			{
				return 0;
			}

			if (ReferenceEquals(null, y))
			{
				return 1;
			}

			if (ReferenceEquals(null, x))
			{
				return -1;
			}

			return x.Version.CompareTo(y.Version);
		}
	}
}