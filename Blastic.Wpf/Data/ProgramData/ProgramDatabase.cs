using Blastic.Data;
using Blastic.Wpf.Data.ProgramData.Migrations;
using Blastic.Wpf.Data.ProgramData.Tables;
using Microsoft.Extensions.Logging;

namespace Blastic.Wpf.Data.ProgramData
{
	public class ProgramDatabase : DatabaseBase<ProgramDatabaseMigrationBase>
	{
		public SettingsTable SettingsTable { get; }

		public ProgramDatabase(
			ConnectionFactory connectionFactory,
			ILogger<ProgramDatabase> logger)
			:
			base(connectionFactory, logger)
		{
			SettingsTable = new SettingsTable(connectionFactory);
		}
	}
}