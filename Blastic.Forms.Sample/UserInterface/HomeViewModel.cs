using System;
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

		private readonly AddMachineForm _addMachineForm;

		public Order Order { get; }
		public IReadOnlyReactiveProperty<string> Title { get; }

		public IReactiveProperty<MachineViewModel> SelectedMachine { get; }
		public ReactiveCollection<MachineViewModel> Machines { get; }

		public AsyncCommand AddMachineCommand { get; }
		public IReadOnlyReactiveProperty<string> AddMachineLabel { get; }

		public HomeViewModel(
			INavigationService navigationService,
			Labels labels)
		{
			_navigationService = navigationService;

			Order = new Order(0);
			Title = labels.Home.Title;

			SelectedMachine = new ReactiveProperty<MachineViewModel>();
			Machines = new ReactiveCollection<MachineViewModel>();

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
		}
	}
}