using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Blastic.Data.ProviderSpecific
{
	public abstract class ProviderSpecifics
	{
		private readonly Connection _connection;

		protected abstract string GetColumnsQueryColumnName { get; }
		protected abstract int MaximumQueryParameter { get; }

		public abstract string IdentityColumn { get; }
		public abstract string NVarCharMaxColumn { get; }
		public abstract string BlobColumn { get; }

		public abstract string TurkishCaseSensitiveCollation { get; }
		public abstract string TurkishCaseInsensitiveCollation { get; }

		public abstract string IgnoreDuplicatesOnIndex { get; }
		public abstract string InsertIgnoringDuplicates { get; }

		public ProviderSpecifics(Connection connection)
		{
			_connection = connection;
		}

		public async Task<bool> TableExists(
			string tableName,
			CancellationToken cancellationToken)
		{
			using Command command = _connection.CreateCommand();

			command.CommandText = GetTableExistsQuery(tableName);
			int count = await command.ExecuteScalar<int>(cancellationToken);

			return count > 0;
		}

		public async Task AlterCollation(
			string tableName,
			string columnName,
			string dataType,
			string collation,
			CancellationToken cancellationToken)
		{
			using Command command = _connection.CreateCommand();
			
			command.CommandText = GetAlterCollationQuery(tableName, columnName, dataType, collation);
			await command.ExecuteNonQuery(cancellationToken);
		}

		public async Task CopyTable(
			string source,
			string destination,
			CancellationToken cancellationToken,
			params string[] columns)
		{
			string columnNames = string.Join(",", columns);

			if (string.IsNullOrEmpty(columnNames))
			{
				columnNames = "*";
			}

			using Command command = _connection.CreateCommand();

			command.CommandText = GetCopyTableQuery(source, destination, columnNames);
			await command.ExecuteNonQuery(cancellationToken);
		}

		public async Task<List<string>> GetColumnNames(
			string tableName,
			CancellationToken cancellationToken)
		{
			List<string> columns = new List<string>();

			using Command command = _connection.CreateCommand();

			command.CommandText = GetColumnNamesQuery(tableName);
			using DataReader reader = await command.ExecuteReader(cancellationToken);

			while (reader.Read())
			{
				string column = reader.Get<string>(GetColumnsQueryColumnName);
				columns.Add(column);
			}

			return columns;
		}

		public async Task DropIndex(
			string tableName,
			string indexName,
			CancellationToken cancellationToken)
		{
			using Command command = _connection.CreateCommand();

			command.CommandText = GetDropIndexQuery(tableName, indexName);
			await command.ExecuteNonQuery(cancellationToken);
		}

		public async Task<DataReader> ExecuteWithPagination(
			Command command,
			int skip,
			int take,
			CancellationToken cancellationToken)
		{
			command.CommandText += GetPaginationQuery(skip, take);
			return await command.ExecuteReader(cancellationToken);
		}

		public async Task<int> ExecuteAndGetInsertedRowId(
			Command command,
			string tableName,
			CancellationToken cancellationToken)
		{
			command.CommandText = $"{command.CommandText.TrimEnd(';', ' ')};{GetInsertedRowIdQuery(tableName)}";
			return await command.ExecuteScalar<int>(cancellationToken);
		}

		public async Task ExecuteWithParameterThrottling<T>(
			Connection connection,
			string baseCommandText,
			string loopCommandText,
			IReadOnlyCollection<T> enumerateOn,
			Func<Command, Task> executeCommand,
			Action<Command> addBaseParameters,
			Action<Command, int> addLoopParameters,
			Action<Command> setBaseParameters,
			Action<Command, T, int> setLoopParameters,
			Action<Command> finalizeCommand)
		{
			using Command command = connection.CreateCommand();

			int baseParameterCount = baseCommandText.Count(x => x == '@');
			int loopParameterCount = loopCommandText.Count(x => x == '@');

			int commitThreshold = MaximumQueryParameter - baseParameterCount;
			commitThreshold /= Math.Max(loopParameterCount, 1);

			int counter = 0;
			int lastCounter = 0;

			addBaseParameters(command);

			for (int i = 0; i < commitThreshold; i++)
			{
				if (i >= enumerateOn.Count)
				{
					break;
				}

				addLoopParameters(command, i);
			}

			setBaseParameters(command);

			async Task Commit()
			{
				// ReSharper disable AccessToDisposedClosure
				// ReSharper disable AccessToModifiedClosure
				if (counter != lastCounter)
				{
					command.CommandText = baseCommandText;

					for (int i = 0; i < counter; i++)
					{
						command.CommandText += loopCommandText.Replace("?", i.ToString());
					}

					finalizeCommand(command);
				}

				await executeCommand(command);

				setBaseParameters(command);

				lastCounter = counter;
				counter = 0;
				// ReSharper restore AccessToModifiedClosure
				// ReSharper restore AccessToDisposedClosure
			}

			foreach (T entry in enumerateOn)
			{
				if (counter == commitThreshold)
				{
					await Commit();
				}

				int parameterIndex = baseParameterCount + loopParameterCount * counter;

				setLoopParameters(command, entry, parameterIndex);

				counter++;
			}

			if (counter > 0)
			{
				int requiredParameterCount = baseParameterCount + counter * loopParameterCount;

				command.RemoveExcessParameters(requiredParameterCount);
				await Commit();
			}
		}

		protected abstract string GetTableExistsQuery(string tableName);
		protected abstract string GetAlterCollationQuery(string tableName, string columnName, string dataType, string collation);
		protected abstract string GetCopyTableQuery(string source, string destination, string columnNames);
		protected abstract string GetColumnNamesQuery(string tableName);
		protected abstract string GetDropIndexQuery(string tableName, string indexName);
		protected abstract string GetPaginationQuery(int skip, int take);
		protected abstract string GetInsertedRowIdQuery(string tableName);
	}
}