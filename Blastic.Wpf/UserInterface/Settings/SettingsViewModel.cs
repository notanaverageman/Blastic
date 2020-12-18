using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Commanding;
using Blastic.Diagnostics;
using Blastic.LifetimeManagement;
using Blastic.LifetimeManagement.Contexts;
using Blastic.Reactive;
using ExecutionContext = Blastic.Execution.ExecutionContext;

namespace Blastic.Wpf.UserInterface.Settings
{
	public class SettingsViewModel : ConductorAllActive<ISettingsSectionViewModel>
	{
		public ExecutionContext ExecutionContext { get; }

		public ReactiveCollection<DiagnosticMessage> DiagnosticMessages { get; set; }

		public TaskCompletionSource<bool>? ShowDiagnosticMessagesTaskCompletionSource { get; set; }
		public ReactiveProperty<bool> IsDiagnosticMessagesVisible { get; set; }

		public Command SaveCommand { get; }
		public Command CancelCommand { get; }

		public Command HideDiagnosticMessagesCommand { get; }
		public Command HideDiagnosticMessagesIgnoreErrorsCommand { get; }
		
		public SettingsViewModel(IEnumerable<ISettingsSectionViewModel> sections)
		{
			ExecutionContext = new ExecutionContext();
			
			DiagnosticMessages = new ReactiveCollection<DiagnosticMessage>();
			IsDiagnosticMessagesVisible = new ReactiveProperty<bool>();

			Items.AddRange(sections);

			SaveCommand = new Command(Save);
			CancelCommand = new Command(Cancel);

			HideDiagnosticMessagesCommand = new Command(HideDiagnosticMessages);
			HideDiagnosticMessagesIgnoreErrorsCommand = new Command(HideDiagnosticMessagesIgnoreErrors);
		}

		public async Task Save(CancellationToken cancellationToken)
		{
			async Task Check(CancellationToken c)
			{
				DiagnosticMessages.Clear();

				foreach (ISettingsSectionViewModel item in Items)
				{
					IEnumerable<DiagnosticMessage> diagnosticMessages = await item.GetDiagnosticMessages(c);
					DiagnosticMessages.AddRange(diagnosticMessages);
				}
			}

			await ExecutionContext.Execute(
				Check,
				"Validating settings.",
				customCancellationToken: cancellationToken);

			if (DiagnosticMessages.Any(x => x.Severity.Value >= Severity.Warning))
			{
				bool shouldContinue = await ShowDiagnosticMessages();

				if (!shouldContinue)
				{
					return;
				}
			}

			await Lifetime.Close(cancellationToken, new ClosureContext(true));
		}

		private Task<bool> ShowDiagnosticMessages()
		{
			// Initialize if the task is null or already completed.
			if (ShowDiagnosticMessagesTaskCompletionSource?.Task?.IsCompleted != false)
			{
				ShowDiagnosticMessagesTaskCompletionSource = new TaskCompletionSource<bool>();
			}

			IsDiagnosticMessagesVisible.Value = true;

			return ShowDiagnosticMessagesTaskCompletionSource.Task;
		}

		public void HideDiagnosticMessages()
		{
			IsDiagnosticMessagesVisible.Value = false;
			ShowDiagnosticMessagesTaskCompletionSource?.SetResult(false);
		}

		public void HideDiagnosticMessagesIgnoreErrors()
		{
			IsDiagnosticMessagesVisible.Value = false;
			ShowDiagnosticMessagesTaskCompletionSource?.SetResult(true);
		}

		public async Task Cancel(CancellationToken cancellationToken)
		{
			await Lifetime.Close(cancellationToken);
		}
	}
}