using System;
using System.Globalization;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Blastic.Common;
using Blastic.Controls.DynamicControls;
using Blastic.Controls.DynamicControls.Elements;
using Blastic.Execution;
using Blastic.LifetimeManagement;
using Blastic.Reactive;
using Blastic.Services.Localization;
using Blastic.Services.Notifications;
using Blastic.UserInterface.TabbedMain;
using Microsoft.Extensions.DependencyInjection;

namespace Blastic.Sample.UserInterface
{
	public class HomeViewModel : Screen, IMainTab
	{
		private readonly ILocalizationService _localizationService;
		private readonly IServiceProvider _serviceProvider;

		public Order Order { get; }
		public bool IsFixed => true;

		public IReactiveProperty<string> Text { get; }
		public IReactiveProperty<string> Title { get; }
		public IReactiveProperty<string> ButtonText { get; }

		public AsyncCommand TestCommand { get; set; }

		public HomeViewModel(
			ExecutionContextFactory executionContextFactory,
			TestSettingsViewModel testSettings,
			ILocalizationService localizationService,
			IServiceProvider serviceProvider)
			:
			base(executionContextFactory)
		{
			_localizationService = localizationService;
			_serviceProvider = serviceProvider;

			Order = new Order(1);

			Text = new ReactiveProperty<string>();
			Title = new LocalizableReactiveProperty(localizationService, "Blastic.Sample.Homepage");
			ButtonText = new LocalizableReactiveProperty(localizationService, "Blastic.Sample.Test");

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
			return Task.CompletedTask;
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

			await ExecutionContext.NotificationService.Enqueue(new Notification(model));
		}
	}
}