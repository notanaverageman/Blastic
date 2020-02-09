using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Commanding;
using Blastic.DynamicControls;
using Blastic.DynamicControls.Properties;
using Blastic.Forms.Sample.Data;
using Blastic.Forms.Services.Navigation;
using Blastic.Forms.UserInterface;
using Blastic.LifetimeManagement;
using Blastic.Ordering;
using Blastic.Reactive;
using Blastic.Services.Localization;

namespace Blastic.Forms.Sample.UserInterface
{
	public class HomeViewModel : Screen, IShellTab
	{
		private readonly INavigationService _navigationService;

		public Order Order { get; }
		public IReadOnlyReactiveProperty<string> Title { get; }

		public ReactiveCollection<Machine> Machines { get; }

		public AsyncCommand AddMachineCommand { get; }

		public HomeViewModel(
			INavigationService navigationService,
			ILocalizationService localizationService,
			ProgramDatabase database)
		{
			_navigationService = navigationService;
			Order = new Order(0);

			Title = new LocalizableReactiveProperty(localizationService, "Sample.Homepage");
			Machines = new ReactiveCollection<Machine>();

			AddMachineCommand = new AsyncCommand()
				.WithSubscribe(async () => await AddMachine(database));

			Lifetime.Initialize.Subscribe(async x =>
			{
				List<Machine> machines = await database.MachinesTable.GetAll(x.Parameter.CancellationToken);
				Machines.AddRange(machines);
			});
		}

		private async Task AddMachine(ProgramDatabase database)
		{
			ReactiveProperty<string> name = new ReactiveProperty<string>("Test");
			ReactiveProperty<int> secondsPerFrame = new ReactiveProperty<int>(4);

			DynamicModel form = new DynamicModel();

			form
				.AddText(name, x => x
                    .WithLabel("Machine name"))
				.AddNumber(secondsPerFrame, x => x
                    .WithLabel("Seconds per frame"))
				.AddGroup(x => x
					.WithHorizontalAlignment(HorizontalAlignment.Stretch)
					.AddAction(
						new AsyncCommand(async () =>
						{
							await _navigationService.GoBack(this);
							form.Ok();
						}),
						y => y
							.WithLabel("Create")
							.WithHorizontalAlignment(HorizontalAlignment.Stretch))
					.AddAction(
						new AsyncCommand(async () =>
						{
							await _navigationService.GoBack(this);
							form.Cancel();
						}),
						y => y
							.WithLabel("Cancel")
							.WithHorizontalAlignment(HorizontalAlignment.Stretch)));

			await _navigationService.NavigateTo(this, form);

			if (!await form.WaitCompletion())
			{
				return;
			}

			Machine machine = new Machine();

			machine.Name.Value = name.Value;
			machine.SecondsPerFrame.Value = secondsPerFrame.Value;

			await database.MachinesTable.Put(machine, CancellationToken.None);

			Machines.Add(machine);
		}
	}
}