using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Commanding;
using Blastic.Initialization;
using Blastic.Initialization.Steps;
using Blastic.LifetimeManagement;
using Blastic.Services.Messaging;
using Blastic.Services.Notifications;
using Blastic.Services.Windowing;
using Blastic.UserInterface.Settings;
using Blastic.Wpf.UserInterface.Events;
using Blastic.Wpf.UserInterface.Logs;

namespace Blastic.Wpf.UserInterface.TabbedMain
{
	public sealed class TabbedMainViewModel : ConductorOneActive<IMainTab>
	{
		private readonly IWindowManager _windowManager;
		private readonly List<IInitializationStep> _initializationSteps;
		private bool _isInitializationStepsRun;

		public INotificationService NotificationService { get; }

		public ProductInformation ProductInformation { get; }
		public LogsViewModel LogsViewModel { get; }
		public SettingsViewModel SettingsViewModel { get; }

		public int FixedHeaderCount { get; }

		public Command ShowLogsCommand { get; }
		public Command ShowSettingsCommand { get; }

		public TabbedMainViewModel(
			IEventAggregator eventAggregator,
			IWindowManager windowManager,
			INotificationService notificationService,
			IEnumerable<IMainTab> mainTabs,
			IEnumerable<IInitializationStep> initializationSteps,
			ProductInformation productInformation = null,
			LogsViewModel logsViewModel = null,
			SettingsViewModel settingsViewModel = null)
		{
			_windowManager = windowManager;
			NotificationService = notificationService;
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

			Items.AddRange(tabs);

			ActiveItem.Value = Items.FirstOrDefault();

			Lifetime.Initialization.Subscribe(async x =>
			{
				await ExecuteInitializationSteps(x);
			});

			Lifetime.Activation.Subscribe(async x =>
			{
				await Activate(ActiveItem.Value, x);
			});

			eventAggregator.SubscribeOnUIThread<OpenLogsEvent>(async _ => await ShowLogs());
			eventAggregator.SubscribeOnUIThread<OpenSettingsEvent>(async _ => await ShowSettings());
			eventAggregator.SubscribeOnUIThread<OpenTabEvent>(async x => await OpenTab(x));

			ShowLogsCommand = new Command().WithSubscribe(ShowLogs);
			ShowSettingsCommand = new Command().WithSubscribe(ShowSettings);
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
				await _windowManager.ShowWindow(LogsViewModel);
			}
		}

		public async Task ShowSettings()
		{
			if (SettingsViewModel != null)
			{
				await _windowManager.ShowWindow(SettingsViewModel);
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