using System.Collections.Generic;
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
			ILogger<ProgramDatabase> logger,
            IEnumerable<ProgramDatabaseMigrationBase> migrations)
			:
			base(connectionFactory, logger, migrations)
		{
			SettingsTable = new SettingsTable(connectionFactory);
		}
	}
}