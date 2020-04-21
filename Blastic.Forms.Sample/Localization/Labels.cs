using System;
using Blastic.Reactive;
using Blastic.Services.Localization;

namespace Blastic.Forms.Sample.Localization
{
	public class Labels
	{
		private readonly ILocalizationService _localizationService;

		public IReadOnlyReactiveProperty<string> Create { get; }
		public IReadOnlyReactiveProperty<string> Cancel { get; }

		public HomeLabels Home { get; }
		public AddMachineLabels AddMachine { get; }

		public MachinesLabels Machines { get; }
		public AddJobLabels AddJob { get; }

		public Labels(ILocalizationService localizationService)
		{
			_localizationService = localizationService;

			Create = CreateProperty("Sample.Create");
			Cancel = CreateProperty("Sample.Cancel");

			Home = new HomeLabels(CreateProperty);
			AddMachine = new AddMachineLabels(CreateProperty);

			Machines = new MachinesLabels(CreateProperty);
			AddJob = new AddJobLabels(CreateProperty);
		}

		private IReactiveProperty<string> CreateProperty(string key)
		{
			return new LocalizableReactiveProperty(_localizationService, key);
		}

		public class HomeLabels
		{
			public IReadOnlyReactiveProperty<string> Title { get; }
			public IReadOnlyReactiveProperty<string> AddMachine { get; }

			public HomeLabels(Func<string, IReadOnlyReactiveProperty<string>> createProperty)
			{
				Title = createProperty("Sample.Home.Title");
				AddMachine = createProperty("Sample.Home.AddMachine");
			}
		}

		public class AddMachineLabels
		{
			public IReadOnlyReactiveProperty<string> MachineName { get; }
			public IReadOnlyReactiveProperty<string> SecondsPerFrame { get; }
			public IReadOnlyReactiveProperty<string> AlreadyExists { get; }

			public AddMachineLabels(Func<string, IReadOnlyReactiveProperty<string>> createProperty)
			{
				MachineName = createProperty("Sample.AddMachine.MachineName");
				SecondsPerFrame = createProperty("Sample.AddMachine.SecondsPerFrame");
				AlreadyExists = createProperty("Sample.AddMachine.AlreadyExists");
			}
		}

		public class MachinesLabels
		{
			public IReadOnlyReactiveProperty<string> AddJob { get; }

			public MachinesLabels(Func<string, IReadOnlyReactiveProperty<string>> createProperty)
			{
				AddJob = createProperty("Sample.Machines.AddJob");
			}
		}

		public class AddJobLabels
		{
			public IReadOnlyReactiveProperty<string> SceneName { get; }
			public IReadOnlyReactiveProperty<string> StartFrame { get; }
			public IReadOnlyReactiveProperty<string> EndFrame { get; }

			public AddJobLabels(Func<string, IReadOnlyReactiveProperty<string>> createProperty)
			{
				SceneName = createProperty("Sample.AddJob.SceneName");
				StartFrame = createProperty("Sample.AddJob.StartFrame");
				EndFrame = createProperty("Sample.AddJob.EndFrame");
			}
		}
	}
}