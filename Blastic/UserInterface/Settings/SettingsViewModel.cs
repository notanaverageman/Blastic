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

		public AsyncCommand SaveCommand { get; }
		public AsyncCommand CancelCommand { get; }

		public Command HideDiagnosticMessagesCommand { get; }
		public Command HideDiagnosticMessagesIgnoreErrorsCommand { get; }
		
		public SettingsViewModel(IEnumerable<ISettingsSectionViewModel> sections)
		{
			DiagnosticMessages = new ReactiveCollection<DiagnosticMessage>();
			IsDiagnosticMessagesVisible = new ReactiveProperty<bool>();

			Items.AddRange(sections);

			SaveCommand = new AsyncCommand(Save);
			CancelCommand = new AsyncCommand(Cancel);

			HideDiagnosticMessagesCommand = new Command(HideDiagnosticMessages);
			HideDiagnosticMessagesIgnoreErrorsCommand = new Command(HideDiagnosticMessagesIgnoreErrors);
		}

		public async Task Save(CommandContext context)
		{
			async Task Check(CancellationToken cancellationToken)
			{
				DiagnosticMessages.Clear();

				foreach (ISettingsSectionViewModel item in Items)
				{
					IEnumerable<DiagnosticMessage> diagnosticMessages = await item.GetDiagnosticMessages(cancellationToken);
					DiagnosticMessages.AddRange(diagnosticMessages);
				}
			}

			await ExecutionContext.Execute(Check, "Validating settings.");

			if (DiagnosticMessages.Any(x => x.Severity.Value >= Severity.Warning))
			{
				bool shouldContinue = await ShowDiagnosticMessages();

				if (!shouldContinue)
				{
					return;
				}
			}

			CommandContext<ClosureContext> commandContext = new CommandContext<ClosureContext>(
				new ClosureContext(true),
				context.CancellationToken);

			await Lifetime.Close.Execute(commandContext);
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

		public async Task Cancel(CommandContext context)
		{
			CommandContext<ClosureContext> commandContext = new CommandContext<ClosureContext>(
				new ClosureContext(),
				context.CancellationToken);

			await Lifetime.Close.Execute(commandContext);
		}
	}
}