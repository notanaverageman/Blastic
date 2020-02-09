using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Blastic.Data.Migrations;
using Blastic.Data.Tables;
using Microsoft.Extensions.Logging;
using Version = Blastic.Ordering.Version;

namespace Blastic.Data
{
	public abstract class DatabaseBase<T> where T : MigrationBase
	{
        private readonly List<MigrationBase> _migrations;

        protected ILogger<DatabaseBase<T>> Logger { get; }
		
		public DatabaseInformationTable DatabaseInformationTable { get; }
		public ConnectionFactory ConnectionFactory { get; }

		protected DatabaseBase(
            ConnectionFactory connectionFactory,
            ILogger<DatabaseBase<T>> logger,
            IEnumerable<T> migrations)
		{
            ConnectionFactory = connectionFactory;
			Logger = logger;

            _migrations = migrations.Cast<MigrationBase>().ToList();
            _migrations.Insert(0, new CreateDatabaseInformationTable());

            DatabaseInformationTable = new DatabaseInformationTable(connectionFactory);
		}

		public TransactionScope CreateTransactionScope()
		{
			return new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
		}

		public async Task<bool> IsMigrationAvailable(CancellationToken cancellationToken)
		{
			using Connection connection = ConnectionFactory.CreateConnection();

			Version currentVersion = await DatabaseInformationTable.GetVersion(connection, cancellationToken);
			Version newVersion = _migrations.Max(x => x.Version);

			return currentVersion != newVersion;
		}

		public async Task Migrate(CancellationToken cancellationToken, Version targetVersion = null)
		{
			using TransactionScope transactionScope = CreateTransactionScope();
			using Connection connection = ConnectionFactory.CreateConnection();
			using IDisposable _ = Logger.BeginScope("Applying migrations.");

			Version currentVersion = await DatabaseInformationTable.GetVersion(connection, cancellationToken);
			Version newVersion = await Migrate(connection, currentVersion, targetVersion, cancellationToken);

			if (currentVersion == newVersion)
			{
				transactionScope.Complete();
				return;
			}

			await DatabaseInformationTable.SetVersion(connection, newVersion, cancellationToken);
			transactionScope.Complete();

			Logger.LogInformation("Finished migrations. New version: {0}", newVersion);
		}

		private async Task<Version> Migrate(
			Connection connection,
			Version currentVersion,
			Version targetVersion,
			CancellationToken cancellationToken)
		{
			IEnumerable<MigrationBase> migrations = _migrations;

			targetVersion ??= migrations.Max(x => x.Version);

			Logger.LogInformation("Current version: {0}. Target version: {1}", currentVersion, targetVersion);

			if (currentVersion == targetVersion)
			{
				return targetVersion;
			}

			Func<MigrationBase, Connection, CancellationToken, Task> migrationFunction;
			Version result;

			if (currentVersion == null || currentVersion < targetVersion)
			{
				migrations = migrations
					.Where(x => x.Version > currentVersion)
					.Where(x => x.Version <= targetVersion)
					.OrderBy(x => x.Version)
					.ToArray();

				migrationFunction = (x, y, z) => x.MigrateUp(y, z);
				result = migrations.Last().Version;
			}
			else
			{
				migrations = migrations
					.Where(x => x.Version <= currentVersion)
					.Where(x => x.Version > targetVersion)
					.OrderByDescending(x => x.Version)
					.ToArray();

				migrationFunction = (x, y, z) => x.MigrateDown(y, z);
				result = migrations.First().Version;
			}

			foreach (MigrationBase migration in migrations)
			{
				await migrationFunction(migration, connection, cancellationToken);
			}

			return result;
		}
	}
}