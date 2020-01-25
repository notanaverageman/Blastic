using Blastic.Data;
using Blastic.Forms.Data.ProgramData.Migrations;
using Blastic.Forms.Data.ProgramData.Tables;
using Microsoft.Extensions.Logging;

namespace Blastic.Forms.Data.ProgramData
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