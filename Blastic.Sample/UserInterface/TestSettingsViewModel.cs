using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Diagnostics;
using Blastic.Execution;
using Blastic.Services.Dialog;
using Blastic.Services.Settings;
using Blastic.UserInterface.Settings;

namespace Blastic.Sample.UserInterface
{
	public class TestSettingsViewModel : SettingsSectionViewModel
	{
		public override string SectionName => "Program";

		public FolderSetting FolderSetting { get; }

		public TestSettingsViewModel(
			ExecutionContextFactory executionContextFactory,
			ISettingsService settingsService,
			IDialogService dialogService)
			:
			base(executionContextFactory, settingsService)
		{
			FolderSetting = new FolderSetting(settingsService, dialogService);
			RegisterForUI(FolderSetting);
		}

		public override Task<IEnumerable<DiagnosticMessage>> GetDiagnosticMessages(CancellationToken cancellationToken)
		{
			IEnumerable<DiagnosticMessage> diagnosticMessages = new[]
				{
					FolderSetting.CheckError(),
				}
				.Select(x => this[x])
				.Where(x => !string.IsNullOrEmpty(x))
				.Select(x => new DiagnosticMessage(Severity.Error, x));

			return Task.FromResult(diagnosticMessages);
		}
	}
}