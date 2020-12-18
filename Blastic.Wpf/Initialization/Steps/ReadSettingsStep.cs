using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Initialization.Steps;
using Blastic.Ordering;
using Blastic.Wpf.UserInterface.Settings;

namespace Blastic.Wpf.Initialization.Steps
{
	public class ReadSettingsStep : IInitializationStep
	{
		public static readonly Order Order = new(-1);
		
		private readonly IEnumerable<ISettingsSectionViewModel> _sections;

		Order IInitializationStep.Order => Order;

		public string Description { get; }
		public string SuccessMessage { get; }
		public string FailureMessage { get; }

		public bool IsCancellationSupported => false;
		public bool ShowBusyIndicator => true;

		public ReadSettingsStep(IEnumerable<ISettingsSectionViewModel> sections)
		{
			_sections = sections;

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
			foreach (ISettingsSectionViewModel section in _sections)
			{
				await section.Lifetime.Initialize(cancellationToken);
			}
		}
	}
}