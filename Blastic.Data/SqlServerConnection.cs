using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Transactions;
using Blastic.Data.ProviderSpecific;
using Microsoft.Extensions.Logging;

namespace Blastic.Data
{
	internal class SqlServerConnection : Connection
	{
		private readonly DatabaseConfiguration _databaseConfiguration;

		protected override DbConnection DbConnection { get; }
		protected override DbTransaction DbTransaction { get; }

		public override DatabaseProvider Provider => DatabaseProvider.SQLServer;
		public override ProviderSpecifics ProviderSpecifics { get; }

		public SqlServerConnection(DatabaseConfiguration databaseConfiguration, ILogger logger) : base(logger)
		{
			_databaseConfiguration = databaseConfiguration;
			ProviderSpecifics = new SqlServerProviderSpecifics(this);

			(DbConnection, DbTransaction) = CreateDbConnection();
		}

		private (DbConnection connection, DbTransaction transaction) CreateDbConnection()
		{
			Logger.LogDebug("Creating new Sql Server connection.");

			DbConnection dbConnection = new SqlConnection(_databaseConfiguration.ConnectionString);
			DbTransaction dbTransaction = null;

			dbConnection.Open();

			if (Transaction.Current == null)
			{
				dbTransaction = dbConnection.BeginTransaction();
			}

			Logger.LogDebug("Started new Sql Server transaction.");

			return (dbConnection, dbTransaction);
		}

		public override void Dispose()
		{
			try
			{
				Logger.LogDebug("Sql Server connection committing transaction.");
				DbTransaction?.Commit();
			}
			catch (Exception exception)
			{
				try
				{
					Logger.LogError(exception, "Sql Server connection transaction has failed. Trying to rollback.");
					DbTransaction?.Rollback();
				}
				catch (Exception)
				{
					// Do nothing here; transaction is not active.
				}
			}

			DbTransaction?.Dispose();
			DbConnection.Dispose();
		}
	}
}