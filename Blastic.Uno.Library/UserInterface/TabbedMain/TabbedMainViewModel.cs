using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.System;
using Blastic.Commanding;
using Blastic.Controls.DynamicControls;
using Blastic.Controls.DynamicControls.Elements;
using Blastic.Execution;
using Blastic.Initialization;
using Blastic.Initialization.Steps;
using Blastic.LifetimeManagement;
using Blastic.Reactive;
using Blastic.Services.Notifications;
using Blastic.UserInterface.Events;
using Blastic.UserInterface.Logs;
using Blastic.UserInterface.Settings;

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

		public AsyncCommand ShowLogsCommand { get; }
		public AsyncCommand ShowSettingsCommand { get; }

		public AsyncCommand OpenCommand { get; }

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
			OpenCommand = new AsyncCommand()
				.WithSubscribe(async () =>
				{
					//DynamicModel model = new DynamicModel();

					//model
					//	.AddText(new ReactiveProperty<string>("Name"))
					//	.AddText(new ReactiveProperty<string>("Surname"))
					//	.AddText(new ReactiveProperty<string>("Company"))
					//	.AddAction(
					//		new Command()
					//			.WithSubscribe(() =>
					//			{
					//				ExecutionContext.Form.Value?.Cancel();
					//			})
					//			.WithKeyboardAccelerator(VirtualKey.F3, VirtualKeyModifiers.Control, model),
					//		x => x
					//			.WithLabel("Close")
					//			.WithMargin(new Thickness(0, 32, 0, 0)));

					//await ExecutionContext.ShowForm(model);

					await ExecutionContext.NotificationService.Enqueue(new Notification(
						new DynamicModel()
							.AddText(new ReactiveProperty<string>("Name"))
							.AddAction(new Command(), x => x
								.WithLabel("Test")),
						TimeSpan.FromMinutes(10)));
				})
				.WithKeyboardAccelerator(VirtualKey.W, VirtualKeyModifiers.Control, this);

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

			Lifetime.Initialize.Subscribe(async x =>
			{
				await ExecuteInitializationSteps(x.Parameter.CancellationToken);
			});

			Lifetime.Activate.Subscribe(async x =>
			{
				await Activate(ActiveItem.Value, x.Parameter.CancellationToken);
			});

			ExecutionContext.EventAggregator.SubscribeOnUIThread<OpenLogsEvent>(async _ => await ShowLogs());
			ExecutionContext.EventAggregator.SubscribeOnUIThread<OpenTabEvent>(async x => await OpenTab(x));

			ShowLogsCommand = new AsyncCommand().WithSubscribe(ShowLogs);
			ShowSettingsCommand = new AsyncCommand().WithSubscribe(ShowSettings);
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