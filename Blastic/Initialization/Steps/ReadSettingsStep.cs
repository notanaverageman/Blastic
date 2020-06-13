using System.Threading;
using System.Threading.Tasks;
using Blastic.LifetimeManagement.Contexts;
using Blastic.Ordering;
using Blastic.UserInterface.Settings;

namespace Blastic.Initialization.Steps
{
	public class ReadSettingsStep : IInitializationStep
	{
		public static readonly Order Order = new Order(-1);

		private readonly SettingsViewModel _settingsViewModel;

		Order IInitializationStep.Order => Order;

		public string Description { get; }
		public string SuccessMessage { get; }
		public string FailureMessage { get; }

		public bool IsCancellationSupported => false;
		public bool ShowBusyIndicator => true;

		public ReadSettingsStep(SettingsViewModel settingsViewModel)
		{
			_settingsViewModel = settingsViewModel;

			Description = "Reading settings...";
			SuccessMessage = "";
			FailureMessage = "Cannot read settings. Program might behave incorrectly.";
		}

		public Task<bool> ShouldExecute(CancellationToken cancellationToken)
		{
			return Task.FromResult(true);
		}

		public async Task Execute(CancellationToken cancellationToken)
		{
			InitializationContext context = new InitializationContext(cancellationToken);
			await _settingsViewModel.Lifetime.Initialize.Execute(context);
		}
	}
}