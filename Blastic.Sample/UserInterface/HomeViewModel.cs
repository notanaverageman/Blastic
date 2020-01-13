using System;
using System.Globalization;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Blastic.Commanding;
using Blastic.Common;
using Blastic.Controls.DynamicControls;
using Blastic.Controls.DynamicControls.Elements;
using Blastic.Execution;
using Blastic.LifetimeManagement;
using Blastic.Reactive;
using Blastic.Sample.Automation;
using Blastic.Services.Localization;
using Blastic.Services.Notifications;
using Blastic.UserInterface.TabbedMain;
using InputSimulatorStandard;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Blastic.Sample.UserInterface
{
	public class HomeViewModel : Screen, IMainTab
	{
		private readonly ILocalizationService _localizationService;
		private readonly IServiceProvider _serviceProvider;
		private readonly ILogger<HomeViewModel> _logger;

		public Order Order { get; }
		public bool IsFixed => true;

		public IReactiveProperty<string> Text { get; }
		public IReactiveProperty<string> Title { get; }
		public IReactiveProperty<string> ButtonText { get; }

		public AsyncCommand TestCommand { get; }
		public AsyncCommand HelpCommand { get; }

		public HomeViewModel(
			ExecutionContextFactory executionContextFactory,
			TestSettingsViewModel testSettings,
			ILocalizationService localizationService,
			IServiceProvider serviceProvider,
			ILogger<HomeViewModel> logger)
			:
			base(executionContextFactory)
		{
			_localizationService = localizationService;
			_serviceProvider = serviceProvider;
			_logger = logger;

			Order = new Order(1);

			Text = new ReactiveProperty<string>();
			Title = new LocalizableReactiveProperty(localizationService, "Blastic.Sample.Homepage");
			ButtonText = new LocalizableReactiveProperty(localizationService, "Blastic.Sample.Test");

			HelpCommand = InitializeHelpCommand();

			TestCommand = Text.HasErrorObservable
				.Select(x => !x)
				.CombineLatest(Lifetime.IsActive, (x, y) => x && y)
				.ToAsyncCommand()
				.WithSubscribe(_ => Test());

			testSettings.FolderSetting.ReactiveValue.Subscribe(x => Text.Value = x);

			Lifetime.Initialize.Subscribe(_ => OnInitialize());
			Lifetime.Activate.Subscribe(_ => OnActivate());

			Text.AddValidator(x => x?.Length > 4 ? "" : "Length is not valid.");
			Text.AddValidator(x => x?.StartsWith("A") == true ? "" : "Does not start with A.");
		}

		protected Task OnInitialize()
		{
			TabbedMainViewModel mainViewModel = _serviceProvider.GetService<TabbedMainViewModel>();
			TestCommand.AddInputGesture(new KeyGesture(Key.A, ModifierKeys.Control), mainViewModel);

			Text.Value = "Initialized";
			return Task.CompletedTask;
		}

		protected Task OnActivate()
		{
			Text.Value = "Activated";

			Application.Current.Dispatcher.BeginInvoke(new Action(async () =>
			{
				await this.MoveMouseTo(HelpCommand);
				new InputSimulator().Mouse.LeftButtonClick();
			}));

			_logger.LogTrace("Activated");
			_logger.LogDebug("Activated");
			_logger.LogInformation("Activated");
			_logger.LogWarning("Activated");
			_logger.LogError("Activated");
			_logger.LogCritical("Activated");

			return Task.CompletedTask;
		}

		private AsyncCommand InitializeHelpCommand()
		{
			return Lifetime.IsActive.ToAsyncCommand().WithSubscribe(async () =>
			{
				await Text.SetText("Some text", TimeSpan.FromSeconds(1.0));
				await this.SetSelection(Text, 0, 6);
			});
		}

		private bool _x;

		public async Task Test()
		{
			_localizationService.SetCulture(_x
				? CultureInfo.GetCultureInfo("en-US")
				: CultureInfo.GetCultureInfo("tr-TR"));

			_x = !_x;

			ExecutionContext.NotificationService.MaximumActiveNotificationCount = 2;

			DynamicModel model = new DynamicModel()
				.AddLabel(new ReactiveProperty<string>("Label"))
				.AddText(Text, x => x
					.WithLabel("Text"))
				.AddAction(TestCommand, x => x
					.WithLabel("Action"));

			await ExecutionContext.NotificationService.Enqueue(new Notification(model, TimeSpan.FromHours(1)));
		}
	}
}