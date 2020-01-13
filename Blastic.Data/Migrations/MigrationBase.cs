using System.Threading;
using System.Threading.Tasks;
using Blastic.Ordering;

namespace Blastic.Data.Migrations
{
	public abstract class MigrationBase
	{
		public abstract Version Version { get; }

		public abstract Task MigrateUp(Connection connection, CancellationToken cancellationToken);
		public abstract Task MigrateDown(Connection connection, CancellationToken cancellationToken);
	}
}