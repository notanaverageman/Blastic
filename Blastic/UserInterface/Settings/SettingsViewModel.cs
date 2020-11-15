using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Commanding;
using Blastic.Diagnostics;
using Blastic.LifetimeManagement;
using Blastic.LifetimeManagement.Contexts;
using Blastic.Reactive;

namespace Blastic.UserInterface.Settings
{
	public class SettingsViewModel : ConductorAllActive<ISettingsSectionViewModel>
	{
		public ReactiveCollection<DiagnosticMessage> DiagnosticMessages { get; set; }

		public TaskCompletionSource<bool>? ShowDiagnosticMessagesTaskCompletionSource { get; set; }
		public ReactiveProperty<bool> IsDiagnosticMessagesVisible { get; set; }

		public Command SaveCommand { get; }
		public Command CancelCommand { get; }

		public Command HideDiagnosticMessagesCommand { get; }
		public Command HideDiagnosticMessagesIgnoreErrorsCommand { get; }
		
		public SettingsViewModel(IEnumerable<ISettingsSectionViewModel> sections)
		{
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

			ClosureContext context = new ClosureContext(true);

			await Lifetime.Close.Execute(context, cancellationToken);
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
			ClosureContext context = new ClosureContext();

			await Lifetime.Close.Execute(context, cancellationToken);
		}
	}
}