using System;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Transactions;
using Blastic.Data.Context;
using Blastic.Data.ProviderSpecific;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace Blastic.Data
{
	internal class SQLiteConnection : Connection, IEnlistmentNotification
	{
		private static readonly ConditionalWeakTable<Transaction, Tuple<DbConnection, DbTransaction>> AmbientConnectionsTable;

		static SQLiteConnection()
		{
			AmbientConnectionsTable = new ConditionalWeakTable<Transaction, Tuple<DbConnection, DbTransaction>>();
		}

		private readonly DatabaseConfiguration _databaseConfiguration;
		private readonly SQLiteConnection _parent;

		private readonly AmbientContext<SQLiteConnection> _ambientContext;

		private Transaction _scopeTransaction;
		private bool _isInTransactionScope;

		protected override DbConnection DbConnection { get; }
		protected override DbTransaction DbTransaction { get; }

		public override DatabaseProvider Provider => DatabaseProvider.SQLite;
		public override ProviderSpecifics ProviderSpecifics { get; }

		public SQLiteConnection(DatabaseConfiguration databaseConfiguration, ILogger logger) : base(logger)
		{
			_databaseConfiguration = databaseConfiguration;

			_ambientContext = new AmbientContext<SQLiteConnection>();

			ProviderSpecifics = new SqliteProviderSpecifics(this);

			RegisterToTransactionScope();

			SQLiteConnection parent = _ambientContext.Get();

			if (parent != null)
			{
				_parent = parent;

				DbConnection = parent.DbConnection;
				DbTransaction = parent.DbTransaction;
			}
			else
			{
				(DbConnection, DbTransaction) = CreateDbConnection();

				_ambientContext.Save(this);
			}
		}

		private void RegisterToTransactionScope()
		{
			if (ShouldRegisterToTransactionScope())
			{
				Logger.LogDebug("SQLite connection registering to transaction scope.");
				Transaction.Current.EnlistVolatile(this, EnlistmentOptions.None);
			}

			_isInTransactionScope = Transaction.Current != null;
			_scopeTransaction = Transaction.Current;
		}

		private (DbConnection connection, DbTransaction transaction) CreateDbConnection()
		{
			if (Transaction.Current != null && AmbientConnectionsTable.TryGetValue(Transaction.Current, out Tuple<DbConnection, DbTransaction> tuple))
			{
				Logger.LogDebug("SQLite connection reusing the connection and transaction from ambient context.");
				return (tuple.Item1, tuple.Item2);
			}

			Logger.LogDebug("SQLite connection opening new db connection.");

			DbConnection dbConnection = new SqliteConnection(_databaseConfiguration.ConnectionString);

			dbConnection.Open();
			DbTransaction dbTransaction = dbConnection.BeginTransaction();

			if (Transaction.Current != null)
			{
				Logger.LogDebug("SQLite connection registering connection and transaction to ambient context.");
				AmbientConnectionsTable.Add(Transaction.Current, new Tuple<DbConnection, DbTransaction>(dbConnection, dbTransaction));
			}

			return (dbConnection, dbTransaction);
		}

		private bool ShouldRegisterToTransactionScope()
		{
			if (Transaction.Current == null)
			{
				return false;
			}

			return !AmbientConnectionsTable.TryGetValue(Transaction.Current, out _);
		}

		private void UnregisterConnection(Transaction transaction)
		{
			if (transaction == null)
			{
				return;
			}

			Logger.LogDebug("SQLite connection unregistering connection and transaction from ambient context.");
			AmbientConnectionsTable.Remove(transaction);
		}

		public override void Dispose()
		{
			_ambientContext.Dispose();

			if (_parent != null)
			{
				Logger.LogDebug("SQLite connection ignoring dispose since parent will handle it.");
				return;
			}

			if (_isInTransactionScope)
			{
				Logger.LogDebug("SQLite connection ignoring dispose since it's in transaction scope and it will handle disposal.");
				return;
			}

			try
			{
				Logger.LogDebug("SQLite connection committing transaction.");
				DbTransaction.Commit();
			}
			catch (Exception exception)
			{
				try
				{
					Logger.LogError(exception, "SQLite connection transaction has failed. Trying to rollback.");
					DbTransaction.Rollback();
				}
				catch (Exception)
				{
					// Do nothing here; transaction is not active.
				}
			}

			Logger.LogDebug("SQLite connection disposing transaction and connection.");

			DbTransaction.Dispose();
			DbConnection.Dispose();
		}

		void IEnlistmentNotification.Commit(Enlistment enlistment)
		{
			Logger.LogDebug("SQLite connection committing transaction through transaction scope enlistment.");

			DbTransaction?.Commit();

			DbTransaction?.Dispose();
			DbConnection.Dispose();

			UnregisterConnection(_scopeTransaction);

			enlistment.Done();
		}

		void IEnlistmentNotification.Rollback(Enlistment enlistment)
		{
			Logger.LogDebug("SQLite connection rolling back transaction through transaction scope enlistment.");

			DbTransaction?.Rollback();
			DbTransaction?.Dispose();

			DbConnection.Close();
			DbConnection.Dispose();

			UnregisterConnection(_scopeTransaction);

			enlistment.Done();
		}

		void IEnlistmentNotification.Prepare(PreparingEnlistment preparingEnlistment)
		{
			preparingEnlistment.Prepared();
		}

		void IEnlistmentNotification.InDoubt(Enlistment enlistment)
		{
			enlistment.Done();
		}
	}
}