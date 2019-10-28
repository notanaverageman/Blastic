using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Blastic.Diagnostics;
using Blastic.Execution;
using Blastic.LifetimeManagement;
using Blastic.LifetimeManagement.Contexts;
using Caliburn.Micro;
using Reactive.Bindings;

namespace Blastic.UserInterface.Settings
{
	public sealed class SettingsViewModel : ConductorAllActive
	{
		private bool _hasReadSettings;
		private Window _activeWindow;

		public IObservableCollection<DiagnosticMessage> DiagnosticMessages { get; set; }

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
			DiagnosticMessages = new BindableCollection<DiagnosticMessage>();
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
			if (_activeWindow != null && PresentationSource.FromVisual(_activeWindow) != null)
			{
				_activeWindow.Activate();
				return;
			}

			dynamic settings = new ExpandoObject();
			settings.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			settings.Owner = Application.Current.MainWindow;

			await ExecutionContext.WindowManager.ShowWindowAsync(this, null, settings);
		}

		// TODO:
		//protected override void OnViewAttached(object view, object context)
		//{
		//	_activeWindow = view as Window;
		//}

		//public override object GetView(object context = null)
		//{
		//	return _activeWindow;
		//}
	}
}