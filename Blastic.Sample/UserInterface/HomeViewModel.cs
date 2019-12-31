using System;
using System.Globalization;
using System.Threading.Tasks;
using Blastic.Common;
using Blastic.Controls.DynamicControls;
using Blastic.Controls.DynamicControls.Elements;
using Blastic.Execution;
using Blastic.LifetimeManagement;
using Blastic.Reactive;
using Blastic.Services.Localization;
using Blastic.Services.Notifications;
using Blastic.UserInterface.TabbedMain;

namespace Blastic.Sample.UserInterface
{
	public class HomeViewModel : Screen, IMainTab
	{
		private readonly ILocalizationService _localizationService;

		public Order Order { get; }
		public bool IsFixed => true;

		public IReactiveProperty<string> Text { get; }
		public IReactiveProperty<string> Title { get; }
		public IReactiveProperty<string> ButtonText { get; }

		public AsyncCommand TestCommand { get; set; }

		public HomeViewModel(
			ExecutionContextFactory executionContextFactory,
			TestSettingsViewModel testSettings,
			ILocalizationService localizationService)
			:
			base(executionContextFactory)
		{
			_localizationService = localizationService;

			Order = new Order(1);

			Text = new ReactiveProperty<string>();
			Title = new LocalizableReactiveProperty(localizationService, "Blastic.Sample.Homepage");
			ButtonText = new LocalizableReactiveProperty(localizationService, "Blastic.Sample.Test");

			TestCommand = new AsyncCommand().WithSubscribe(_ => Test());

			testSettings.FolderSetting.ReactiveValue.Subscribe(x => Text.Value = x);

			Lifetime.Initialize.Subscribe(_ => OnInitialize());
			Lifetime.Activate.Subscribe(_ => OnActivate());
		}

		protected Task OnInitialize()
		{
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
				.AddAction(TestCommand, x => x
					.WithLabel("Action"))
				.AddLabel(new ReactiveProperty<string>("Label"))
				.AddText(Text);

			await ExecutionContext.NotificationService.Enqueue(new Notification(model));
		}
	}
}