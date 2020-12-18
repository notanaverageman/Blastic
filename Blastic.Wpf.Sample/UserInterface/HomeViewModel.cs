using System;
using System.Globalization;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Blastic.Commanding;
using Blastic.DynamicControls;
using Blastic.LifetimeManagement;
using Blastic.Ordering;
using Blastic.Reactive;
using Blastic.Services.Localization;
using Blastic.Services.Notifications;
using Blastic.ViewManagement;
using Blastic.Wpf.Automation;
using Blastic.Wpf.Commanding;
using Blastic.Wpf.UserInterface.TabbedMain;
using InputSimulatorStandard;
using Microsoft.Extensions.Logging;

namespace Blastic.Wpf.Sample.UserInterface
{
	public class HomeViewModel : IViewAware, IMainTab
	{
		private readonly INotificationService _notificationService;
		private readonly ILocalizationService _localizationService;
		private readonly ILogger<HomeViewModel> _logger;

		public ILifetime Lifetime { get; }
		public IReactiveProperty<object?> View { get; }
		
		public Order Order { get; }
		public bool IsFixed => true;

		public IReadOnlyReactiveProperty<string> Title { get; }

		public IReactiveProperty<string> Text { get; }
		public IReadOnlyReactiveProperty<string> ButtonText { get; }

		public Command TestCommand { get; }
		public Command HelpCommand { get; }

		public HomeViewModel(
			TestSettingsViewModel testSettings,
			INotificationService notificationService,
			ILocalizationService localizationService,
			ILogger<HomeViewModel> logger)
		{
			_notificationService = notificationService;
			_localizationService = localizationService;
			_logger = logger;

			Lifetime = new Lifetime();
			View = new ReactiveProperty<object?>();
			
			Order = new Order(1);

			Text = new ReactiveProperty<string>();
			Title = new LocalizableReactiveProperty(localizationService, "Blastic.Sample.Homepage");
			ButtonText = new LocalizableReactiveProperty(localizationService, "Blastic.Sample.Test")
				.Select((x, y) => string.Format(x, y))
				.ToReadOnlyReactiveProperty();

			ReadOnlyReactiveProperty<string> lengthErrorMessage = new LocalizableReactiveProperty(_localizationService, "Blastic.Sample.InvalidLength")
				.CombineLatest(Text, (errorMessage, text) => (ErrorMessage: errorMessage, Text: text))
				.Select(x => string.Format(x.ErrorMessage, x.Text?.Length ?? 0, 5))
				.ToReadOnlyReactiveProperty();

			Text.AddValidator(x => x?.Length > 4 ? null : lengthErrorMessage);

			Text.AddValidator(
				x => x?.StartsWith("A") == true
					? null
					: new LocalizableReactiveProperty(_localizationService, "Blastic.Sample.InvalidInitial"));

			HelpCommand = Text.HasErrorObservable!
				.Select(x => !x)
				.CombineLatest(Lifetime.IsActive, (x, y) => x && y)
				.ToCommand()
				.WithSubscribe(async () =>
				{
					await Text.SetText("Some text", TimeSpan.FromSeconds(1.0));
					await this.SetSelection(Text, 0, 6);
				});

			TestCommand = Lifetime.IsActive
				.ToCommand()
				.WithSubscribe(Test);

			testSettings.FolderSetting.ReactiveValue.Subscribe(x => Text.Value = x);

			Lifetime.Initialization.Subscribe(OnInitialize);
			Lifetime.Activation.Subscribe(OnActivate);
		}

		protected Task OnInitialize()
		{
			TestCommand.AddInputGesture(new KeyGesture(Key.A, ModifierKeys.Control));

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

		private bool _x;

		public async Task Test()
		{
			_localizationService.Culture.Value = _x
				? CultureInfo.GetCultureInfo("en-US")
				: CultureInfo.GetCultureInfo("tr-TR");

			_x = !_x;

			_notificationService.MaximumActiveNotificationCount = 2;

			DynamicModel model = new DynamicModel()
				.AddLabel(new ReactiveProperty<string>("Label"))
				.AddText(Text, x => x
					.WithLabel("Text"))
				.AddAction(TestCommand, x => x
					.WithLabel("Action"));

			await _notificationService.Enqueue(new Notification(model, TimeSpan.FromHours(1)));
		}
	}
}