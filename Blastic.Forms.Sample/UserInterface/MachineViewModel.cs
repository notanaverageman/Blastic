using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Commanding;
using Blastic.DynamicControls;
using Blastic.Forms.Sample.Data;
using Blastic.Forms.Sample.Localization;
using Blastic.Forms.Services.Navigation;
using Blastic.LifetimeManagement;
using Blastic.Reactive;

namespace Blastic.Forms.Sample.UserInterface
{
	public class MachineViewModel : Screen
	{
		private readonly INavigationService _navigationService;

		private readonly AddJobForm _addJobForm;

		public Machine Machine { get; }

		public IReactiveProperty<JobViewModel> SelectedJob { get; }
		public ReactiveCollection<JobViewModel> Jobs { get; }

		public Command AddJobCommand { get; }
		public IReadOnlyReactiveProperty<string> AddJobLabel { get; }

		public MachineViewModel(
			INavigationService navigationService,
			Labels labels,
			ProgramDatabase database,
			Machine machine)
		{
			_navigationService = navigationService;

			Machine = machine;

			SelectedJob = new ReactiveProperty<JobViewModel>();
			Jobs = new ReactiveCollection<JobViewModel>();

			Jobs.AddRange(machine.Jobs.Select(x => new JobViewModel(x)));

			AddJobCommand = new Command().WithSubscribe(async () => await AddJob(database));
			AddJobLabel = labels.Machines.AddJob;

			_addJobForm = new AddJobForm(
				labels,
				async () => await _navigationService.GoBack(this));

			SelectedJob.Subscribe(
				x =>
				{
					if (x == null)
					{
						return;
					}

					// We don't want to see items as selected on UI.
					SelectedJob.Value = null;
					navigationService.NavigateTo(this, x);

					DynamicModel model = new DynamicModel();

					model
						.AddLabel(x.Job.SceneName)
						.AddLabel(x.Job.InfoFrames)
						.AddAction(
							new Command(
								async () =>
								{
									x.Job.StartDate.Value = DateTime.Now;
									x.Job.IsStarted.Value = true;

									await database.JobsTable.Put(x.Job, CancellationToken.None);

									await navigationService.GoBack(this);
								}), y => y.WithLabel("Start"));

					navigationService.NavigateTo(this, model);
				});
		}

		private async Task AddJob(ProgramDatabase database)
		{
			await _navigationService.NavigateTo(this, _addJobForm.Form);

			if (!await _addJobForm.Form.WaitCompletion())
			{
				return;
			}

			Job job = new Job(Machine);

			job.SceneName.Value = _addJobForm.SceneName.Value;

			job.StartFrame.Value = _addJobForm.StartFrame.Value;
			job.EndFrame.Value = _addJobForm.EndFrame.Value;

			job.QueueDate.Value = DateTime.Now;

			// We use FileTimeUtc for SQLite and it throws an exception with small values.
			job.StartDate.Value = new DateTime(1980, 1, 1);

			await database.JobsTable.Put(job, CancellationToken.None);

			Jobs.Add(new JobViewModel(job));
		}
	}
}