using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Blastic.Caliburn;
using Blastic.Common;
using Blastic.Controls.DynamicControls;
using Blastic.Controls.DynamicControls.Elements;
using Blastic.Execution;
using Blastic.UserInterface.TabbedMain;
using MaterialDesignThemes.Wpf;
using Reactive.Bindings;

namespace Blastic.Sample.UserInterface
{
	public class HomeViewModel : ScreenBase, IMainTab
	{
		public Order Order { get; }
		public bool IsFixed => true;

		public string Text { get; set; }

		public HomeViewModel(
			ExecutionContextFactory executionContextFactory,
			TestSettingsViewModel testSettings)
			:
			base(executionContextFactory)
		{
			Order = new Order(1);

			testSettings.FolderSetting.ReactiveValue.Subscribe(x => Text = x);
		}

		protected override Task OnInitializeAsync(CancellationToken cancellationToken)
		{
			Text = "Initialized";
			return Task.CompletedTask;
		}

		protected override Task OnActivateAsync(CancellationToken cancellationToken)
		{
			Text = "Activated";
			return Task.CompletedTask;
		}

		public async Task Test()
		{
			ReactiveProperty<string> name = new ReactiveProperty<string>();
			ReactiveProperty<string> password = new ReactiveProperty<string>();
			ReactiveProperty<int> age = new ReactiveProperty<int>();
			ReactiveProperty<bool> boolean = new ReactiveProperty<bool>();
			ReactiveCommand command = new ReactiveCommand(boolean);

			int asd = 0;
			command.Subscribe(() =>
			{
				asd++;
				name.Value = asd.ToString();
			});

			DynamicModel form = new DynamicModel()
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