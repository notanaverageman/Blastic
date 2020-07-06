using System.Threading;
using System.Threading.Tasks;
using Blastic.Initialization.Steps;
using Blastic.Ordering;
using Blastic.Wpf.Sample.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Blastic.Wpf.Sample.Initialization
{
	public class EnsureDatabaseInitializationStep : IInitializationStep
	{
		public static readonly Order Order = new Order(0);

		private readonly SampleContext _dbContext;
		private readonly ILogger<EnsureDatabaseInitializationStep> _logger;

		Order IInitializationStep.Order => Order;

		public string Description { get; }
		public string SuccessMessage { get; }
		public string FailureMessage { get; }

		public bool IsCancellationSupported => false;
		public bool ShowBusyIndicator => true;

		public EnsureDatabaseInitializationStep(
			SampleContext dbContext,
			ILogger<EnsureDatabaseInitializationStep> logger)
		{
			_dbContext = dbContext;
			_logger = logger;

			Description = "Ensuring that the database exists.";
			SuccessMessage = "";
			FailureMessage = "Cannot create database. Program might behave incorrectly.";
		}

		public Task<bool> ShouldExecute(CancellationToken cancellationToken)
		{
			return Task.FromResult(true);
		}

		public async Task Execute(CancellationToken cancellationToken)
		{
			_logger.LogDebug("Checking and applying migrations.");
			await _dbContext.Database.MigrateAsync(cancellationToken);
			_logger.LogDebug("Finished checking and applying migrations.");
		}
	}
}