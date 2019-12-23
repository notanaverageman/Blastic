using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Blastic.Common;
using Blastic.Controls.DynamicControls;
using Blastic.Controls.DynamicControls.Elements;
using Blastic.Execution;
using Blastic.LifetimeManagement;
using Blastic.Reactive;
using Blastic.Services.Localization;
using Blastic.UserInterface.TabbedMain;
using MaterialDesignThemes.Wpf;

namespace Blastic.Sample.UserInterface
{
	public class MainTabViewModel : Screen, IMainTab
	{
		public Order Order { get; }
		public bool IsFixed => true;

		public IReactiveProperty<string> Title { get; }
		public IReactiveProperty<string> Text { get; set; }

		public AsyncCommand TestCommand { get; set; }

		public MainTabViewModel(
			ExecutionContextFactory executionContextFactory,
			TestSettingsViewModel testSettings,
			ILocalizationService localizationService)
			:
			base(executionContextFactory)
		{
			Order = new Order(2);

			Text = new ReactiveProperty<string>();
			Title = new LocalizableReactiveProperty(localizationService, "Blastic.Sample.Homepage");

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

		public async Task Test()
		{
			ReactiveProperty<string> name = new ReactiveProperty<string>();
			ReactiveProperty<string> password = new ReactiveProperty<string>();
			ReactiveProperty<int> age = new ReactiveProperty<int>();
			ReactiveProperty<bool> boolean = new ReactiveProperty<bool>();
			Command command = new Command(boolean);

			int asd = 0;
			command.Subscribe(() =>
			{
				asd++;
				name.Value = asd.ToString();
			});

			DynamicModel form = new DynamicModel()
				.AddLabel(name)
				.AddSelection(age, new ReactiveCollection<int>(Enumerable.Range(1, 20)), x => x
					.WithLabel("Ages")
					.WithIcon(PackIconKind.AbTesting))
				.AddGroup(x => x
					.WithHelp("Some help content.")
					.AddText(name, y => y
						.WithLabel("File path")
						.WithIcon(PackIconKind.FileExcel)
						.WithColumnWidth(new GridLength(1, GridUnitType.Star)))
					.AddAction(command, y => y
						.WithIcon(PackIconKind.Folder)))
				.AddText(name, x => x
					.WithLabel("Name")
					.WithHelp("Name of the user.")
					.WithIcon(PackIconKind.User))
				.AddPassword(password, x => x
					.WithLabel("Password")
					.WithHelp("Password of the user.")
					.WithIcon(PackIconKind.Lock))
				.AddNumber(age, x => x
					.WithLabel("Age")
					.WithHelp("Age of the user.")
					.WithIcon(PackIconKind.JackOLantern))
				.AddBoolean(boolean, x => x
					.WithLabel("Some check")
					.WithHelp("Some check for the user."))
				.AddAction(command, x => x
					.WithLabel("Some Button")
					.WithIcon(PackIconKind.Add)
					.WithIconMargin(new Thickness(0, 0, 8, 0))
					.WithHorizontalAlignment(HorizontalAlignment.Right));

			await ExecutionContext.ShowForm(form);
		}
	}
}