using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Commanding;
using Blastic.Forms.Sample.Data;
using Blastic.Forms.Sample.Localization;
using Blastic.Forms.Services.Navigation;
using Blastic.Forms.UserInterface;
using Blastic.LifetimeManagement;
using Blastic.Ordering;
using Blastic.Reactive;

namespace Blastic.Forms.Sample.UserInterface
{
	public class HomeViewModel : Screen, IShellTab
	{
		private readonly INavigationService _navigationService;
		private readonly Labels _labels;
		private readonly ProgramDatabase _database;

		private readonly AddMachineForm _addMachineForm;

		public Order Order { get; }
		public IReadOnlyReactiveProperty<string> Title { get; }

		public IReactiveProperty<MachineViewModel> SelectedMachine { get; }
		public ReactiveCollection<MachineViewModel> Machines { get; }

		public AsyncCommand AddMachineCommand { get; }
		public IReadOnlyReactiveProperty<string> AddMachineLabel { get; }

		public HomeViewModel(
			INavigationService navigationService,
			Labels labels,
			ProgramDatabase database)
		{
			_navigationService = navigationService;
			_labels = labels;
			_database = database;

			Order = new Order(0);
			Title = labels.Home.Title;

			SelectedMachine = new ReactiveProperty<MachineViewModel>();
			Machines = new ReactiveCollection<MachineViewModel>();

			Lifetime.Initialize.Subscribe(async x =>
			{
				List<Machine> machines = await database.MachinesTable.GetAll(x.Parameter.CancellationToken);
				Machines.AddRange(machines.Select(y => new MachineViewModel(navigationService, labels, database, y)));
			});

			AddMachineCommand = new AsyncCommand().WithSubscribe(async () => await AddMachine());
			AddMachineLabel = labels.Home.AddMachine;

			_addMachineForm = new AddMachineForm(
				labels,
				Machines,
				async () => await _navigationService.GoBack(this));

			SelectedMachine.Subscribe(
				x =>
				{
					if (x == null)
					{
						return;
					}

					// We don't want to see items as selected on UI.
					SelectedMachine.Value = null;

					navigationService.NavigateTo(this, x);
				});
		}

		private async Task AddMachine()
		{
			await _navigationService.NavigateTo(this, _addMachineForm.Form);

			if (!await _addMachineForm.Form.WaitCompletion())
			{
				return;
			}

			Machine machine = new Machine();

			machine.Name.Value = _addMachineForm.MachineName.Value;
			machine.SecondsPerFrame.Value = _addMachineForm.SecondsPerFrame.Value;

			await _database.MachinesTable.Put(machine, CancellationToken.None);

			Machines.Add(new MachineViewModel(_navigationService, _labels, _database, machine));
		}
	}
}