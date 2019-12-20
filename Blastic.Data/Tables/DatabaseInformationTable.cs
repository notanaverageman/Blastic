using System.Threading;
using System.Threading.Tasks;
using Blastic.Common;

namespace Blastic.Data.Tables
{
	public class DatabaseInformationTable : TableBase
	{
		public static string TableName { get; set; } = "DatabaseInformation";

		public DatabaseInformationTable(ConnectionFactory connectionFactory) : base(connectionFactory)
		{
		}

		public async Task<Version> GetVersion(CancellationToken cancellationToken)
		{
			using Connection connection = ConnectionFactory.CreateConnection();
			
			return await GetVersion(connection, cancellationToken);
		}

		public async Task<Version> GetVersion(Connection connection, CancellationToken cancellationToken)
		{
			bool tableExists = await connection.ProviderSpecifics.TableExists(
				TableName,
				cancellationToken);

			if (!tableExists)
			{
				return null;
			}

			using Command command = connection.CreateCommand();

			command.CommandText = $"SELECT Version FROM {TableName}";
			string versionAsString = await command.ExecuteScalar<string>(cancellationToken);

			return Version.Parse(versionAsString);
		}

		public async Task SetVersion(Version version, CancellationToken cancellationToken)
		{
			using Connection connection = ConnectionFactory.CreateConnection();

			await SetVersion(connection, version, cancellationToken);
		}

		public async Task SetVersion(Connection connection, Version version, CancellationToken cancellationToken)
		{
			using Command command = connection.CreateCommand();

			command.CommandText = $"UPDATE {TableName} SET Version=@Version";
			command.AddParameterWithValue("@Version", version.ToString());

			await command.ExecuteNonQuery(cancellationToken);
		}
	}
}