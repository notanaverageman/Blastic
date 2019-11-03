using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Blastic.Common.Diagnostics;
using Blastic.Diagnostics;
using Blastic.Execution;
using Blastic.LifetimeManagement;
using Blastic.LifetimeManagement.Contexts;
using Blastic.ViewManagement;
using Reactive.Bindings;

namespace Blastic.UserInterface.Settings
{
	public sealed class SettingsViewModel : ConductorAllActive<ISettingsSectionViewModel>, IViewAware
	{
		public IReactiveProperty<UIElement> View { get; }

		public ReactiveCollection<DiagnosticMessage> DiagnosticMessages { get; set; }

		public TaskCompletionSource<bool> ShowDiagnosticMessagesTaskCompletionSource { get; set; }
		public ReactiveProperty<bool> IsDiagnosticMessagesVisible { get; set; }

		public AsyncReactiveCommand SaveCommand { get; }
		public AsyncReactiveCommand CancelCommand { get; }

		public ReactiveCommand HideDiagnosticMessagesCommand { get; }
		public ReactiveCommand HideDiagnosticMessagesIgnoreErrorsCommand { get; }
		
		public SettingsViewModel(
			ExecutionContextFactory executionContextFactory,
			IEnumerable<ISettingsSectionViewModel> sections)
			:
			base(executionContextFactory)
		{
			View = new ReactiveProperty<UIElement>();
			DiagnosticMessages = new ReactiveCollection<DiagnosticMessage>();
			IsDiagnosticMessagesVisible = new ReactiveProperty<bool>();

			DisplayName.Value = "Settings";

			foreach (ISettingsSectionViewModel section in sections)
			{
				Items.Add(section);
			}

			SaveCommand = new AsyncReactiveCommand().WithSubscribe(Save);
			CancelCommand = new AsyncReactiveCommand().WithSubscribe(Cancel);

			HideDiagnosticMessagesCommand = new ReactiveCommand().WithSubscribe(HideDiagnosticMessages);
			HideDiagnosticMessagesIgnoreErrorsCommand = new ReactiveCommand().WithSubscribe(HideDiagnosticMessagesIgnoreErrors);
		}

		public async Task Save()
		{
			async Task Check(CancellationToken cancellationToken)
			{
				DiagnosticMessages.Clear();

				foreach (ISettingsSectionViewModel item in Items)
				{
					IEnumerable<DiagnosticMessage> diagnosticMessages = await item.GetDiagnosticMessages(cancellationToken);

					// TODO: AddRange
					foreach (DiagnosticMessage diagnosticMessage in diagnosticMessages)
					{
						DiagnosticMessages.Add(diagnosticMessage);
					}
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
			await ExecutionContext.WindowManager.ShowWindow(this, x =>
			{
				x.WindowStartupLocation = WindowStartupLocation.CenterOwner;
				x.Owner = Application.Current.MainWindow;
			});
		}
	}
}