using System;
using System.Reactive.Linq;
using Blastic.Reactive;

namespace Blastic.Forms.Sample.Data
{
	public class Job
	{
		public int Id { get; set; }

		public IReactiveProperty<Machine> Machine { get; }

		public IReactiveProperty<string> SceneName { get; }
		public IReactiveProperty<bool> IsStarted { get; }

		public IReactiveProperty<DateTime> QueueDate { get; }
		public IReactiveProperty<DateTime> StartDate { get; }

		public IReactiveProperty<int> StartFrame { get; }
		public IReactiveProperty<int> EndFrame { get; }

		public IReadOnlyReactiveProperty<string> InfoFrames { get; }
		public IReadOnlyReactiveProperty<double> InfoProgress { get; }

		public Job(Machine machine) : this(-1, machine)
		{
		}

		public Job(int id, Machine machine)
		{
			Id = id;

			Machine = new ReactiveProperty<Machine>(machine);

			SceneName = new ReactiveProperty<string>();
			IsStarted = new ReactiveProperty<bool>();

			QueueDate = new ReactiveProperty<DateTime>();
			StartDate = new ReactiveProperty<DateTime>();

			StartFrame = new ReactiveProperty<int>();
			EndFrame = new ReactiveProperty<int>();

			InfoFrames = StartFrame
				.CombineLatest(EndFrame, (x, y) => $"{x} - {y}")
				.ToReadOnlyReactiveProperty();

			InfoProgress = Observable
				.Interval(TimeSpan.FromMilliseconds(100))
				.Select(
					_ =>
					{
						if (!IsStarted.Value)
						{
							return 0;
						}

						DateTime now = DateTime.Now;
						DateTime startDate = StartDate.Value;

						TimeSpan elapsed = now - startDate;

						int secondsPerFrame = Machine.Value.SecondsPerFrame.Value;
						int numberOfFrames = EndFrame.Value - StartFrame.Value;

						return (elapsed.TotalSeconds / secondsPerFrame) / numberOfFrames;
					})
				.ToReadOnlyReactiveProperty();
		}
	}
}