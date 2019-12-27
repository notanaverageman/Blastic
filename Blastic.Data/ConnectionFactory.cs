using System;
using Microsoft.Extensions.Logging;

namespace Blastic.Data
{
	public class ConnectionFactory
	{
		private readonly DatabaseConfiguration _databaseConfiguration;
		private readonly ILogger _logger;

		public ConnectionFactory(DatabaseConfiguration databaseConfiguration, ILogger<ConnectionFactory> logger)
		{
			_databaseConfiguration = databaseConfiguration;
			_logger = logger;
		}

		public Connection CreateConnection()
		{
			switch (_databaseConfiguration.DatabaseProvider)
			{
				case DatabaseProvider.SQLite:
					return new SQLiteConnection(_databaseConfiguration, _logger);
				case DatabaseProvider.SQLServer:
					return new SqlServerConnection(_databaseConfiguration, _logger);
				default:
					throw new ArgumentOutOfRangeException(
						nameof(_databaseConfiguration.DatabaseProvider),
						"Database provider is not implemented: " + _databaseConfiguration.DatabaseProvider);
			}
		}
	}
}