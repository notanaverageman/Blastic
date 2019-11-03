using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Execution;
using Blastic.Initialization;
using Blastic.Initialization.Steps;
using Blastic.LifetimeManagement;
using Blastic.UserInterface.Events;
using Blastic.UserInterface.Logs;
using Blastic.UserInterface.Settings;
using Reactive.Bindings;

namespace Blastic.UserInterface.TabbedMain
{
	public sealed class TabbedMainViewModel : ConductorOneActive<IMainTab>
	{
		private readonly List<IInitializationStep> _initializationSteps;
		private bool _isInitializationStepsRun;

		public ProductInformation ProductInformation { get; }
		public LogsViewModel LogsViewModel { get; }
		public SettingsViewModel SettingsViewModel { get; }

		public int FixedHeaderCount { get; }

		public AsyncReactiveCommand ShowLogsCommand { get; }
		public AsyncReactiveCommand ShowSettingsCommand { get; }

		public TabbedMainViewModel(
			ExecutionContextFactory executionContextFactory,
			IEnumerable<IMainTab> mainTabs,
			IEnumerable<IInitializationStep> initializationSteps,
			ProductInformation productInformation = null,
			LogsViewModel logsViewModel = null,
			SettingsViewModel settingsViewModel = null)
			:
			base(executionContextFactory)
		{
			ProductInformation = productInformation;
			LogsViewModel = logsViewModel;
			SettingsViewModel = settingsViewModel;

			_initializationSteps = initializationSteps
				.OrderBy(x => x.Order)
				.ToList();

			List<IMainTab> tabs = mainTabs
				.OrderBy(x => x.Order)
				.ToList();

			FixedHeaderCount = tabs.Count(x => x.IsFixed);

			// TODO: AddRange.
			foreach (IMainTab tab in tabs)
			{
				Items.Add(tab);
			}

			Lifetime.Activate.Subscribe(async x =>
			{
				await Activate(Items.FirstOrDefault(), x.Parameter.CancellationToken);
			});

			Lifetime.Initialize.Subscribe(x => ExecuteInitializationSteps(x.Parameter.CancellationToken));

			ExecutionContext.EventAggregator.SubscribeOnUIThread<OpenLogsEvent>(async _ => await ShowLogs());
			ExecutionContext.EventAggregator.SubscribeOnUIThread<OpenTabEvent>(async x => await OpenTab(x));
			
			ShowLogsCommand = new AsyncReactiveCommand().WithSubscribe(ShowLogs);
			ShowSettingsCommand = new AsyncReactiveCommand().WithSubscribe(ShowSettings);
		}

		private async Task ExecuteInitializationSteps(CancellationToken cancellationToken)
		{
			if (_isInitializationStepsRun)
			{
				return;
			}

			foreach (IInitializationStep initializationStep in _initializationSteps)
			{
				if (!await initializationStep.ShouldExecute(cancellationToken))
				{
					continue;
				}

				await ExecutionContext.Execute(
					initializationStep.Execute,
					initializationStep.Description,
					initializationStep.SuccessMessage,
					initializationStep.FailureMessage,
					initializationStep.ShowBusyIndicator,
					rethrowUnhandledException: false,
					initializationStep.IsCancellationSupported,
					cancellationToken);
			}

			_isInitializationStepsRun = true;
		}

		public async Task ShowLogs()
		{
			if (LogsViewModel != null)
			{
				await LogsViewModel.Show();
			}
		}

		public async Task ShowSettings()
		{
			if (SettingsViewModel != null)
			{
				await SettingsViewModel.Show();
			}
		}
		
		public async Task OpenTab(OpenTabEvent message)
		{
			IMainTab tab = message.ViewModel;

			if (!Items.Contains(tab))
			{
				Items.Add(tab);
			}

			await Activate(tab);
		}
	}
}