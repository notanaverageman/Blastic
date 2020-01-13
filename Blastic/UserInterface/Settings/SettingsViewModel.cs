using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Commanding;
using Blastic.Diagnostics;
using Blastic.LifetimeManagement;
using Blastic.LifetimeManagement.Contexts;
using Blastic.Reactive;
using Blastic.Services.Windowing;

namespace Blastic.UserInterface.Settings
{
	public sealed class SettingsViewModel : ConductorAllActive<ISettingsSectionViewModel>
	{
		private readonly IWindowManager _windowManager;

		public ReactiveCollection<DiagnosticMessage> DiagnosticMessages { get; set; }

		public TaskCompletionSource<bool> ShowDiagnosticMessagesTaskCompletionSource { get; set; }
		public ReactiveProperty<bool> IsDiagnosticMessagesVisible { get; set; }

		public AsyncCommand SaveCommand { get; }
		public AsyncCommand CancelCommand { get; }

		public Command HideDiagnosticMessagesCommand { get; }
		public Command HideDiagnosticMessagesIgnoreErrorsCommand { get; }
		
		public SettingsViewModel(
			IWindowManager windowManager,
			IEnumerable<ISettingsSectionViewModel> sections)
		{
			_windowManager = windowManager;
			DiagnosticMessages = new ReactiveCollection<DiagnosticMessage>();
			IsDiagnosticMessagesVisible = new ReactiveProperty<bool>();

			Items.AddRange(sections);

			SaveCommand = new AsyncCommand(Save);
			CancelCommand = new AsyncCommand(Cancel);

			HideDiagnosticMessagesCommand = new Command(HideDiagnosticMessages);
			HideDiagnosticMessagesIgnoreErrorsCommand = new Command(HideDiagnosticMessagesIgnoreErrors);
		}

		public async Task Save()
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

			if (DiagnosticMessages.Any(x => x.Severity >= Severity.Warning))
			{
				bool shouldContinue = await ShowDiagnosticMessages();

				if (!shouldContinue)
				{
					return;
				}
			}

			ClosureContext context = new ClosureContext(CancellationToken.None)
			{
				DialogResult = true
			};

			await Lifetime.Close.Execute(context);
		}

		private Task<bool> ShowDiagnosticMessages()
		{
			ShowDiagnosticMessagesTaskCompletionSource = new TaskCompletionSource<bool>();
			IsDiagnosticMessagesVisible.Value = true;

			return ShowDiagnosticMessagesTaskCompletionSource.Task;
		}

		public void HideDiagnosticMessages()
		{
			IsDiagnosticMessagesVisible.Value = false;
			ShowDiagnosticMessagesTaskCompletionSource.SetResult(false);
		}

		public void HideDiagnosticMessagesIgnoreErrors()
		{
			IsDiagnosticMessagesVisible.Value = false;
			ShowDiagnosticMessagesTaskCompletionSource.SetResult(true);
		}

		public async Task Cancel()
		{
			ClosureContext context = new ClosureContext(CancellationToken.None)
			{
				DialogResult = false
			};

			await Lifetime.Close.Execute(context);
		}

		public async Task Show()
		{
			await _windowManager.ShowWindow(this);
		}
	}
}