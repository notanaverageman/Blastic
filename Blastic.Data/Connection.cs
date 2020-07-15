using System;
using System.Data.Common;
using Blastic.Data.ProviderSpecific;
using Microsoft.Extensions.Logging;

namespace Blastic.Data
{
	public abstract class Connection : IDisposable
	{
		protected ILogger Logger { get; }

		public abstract DatabaseProvider Provider { get; }
		public abstract ProviderSpecifics ProviderSpecifics { get; }

		protected abstract DbConnection DbConnection { get; }
		protected abstract DbTransaction? DbTransaction { get; }

		public Connection(ILogger logger)
		{
			Logger = logger;
		}

		public Command CreateCommand()
		{
			DbCommand command = DbConnection.CreateCommand();

			if (DbTransaction != null)
			{
				command.Transaction = DbTransaction;
			}

			return new Command(command, Provider, Logger);
		}

		public abstract void Dispose();
	}
}