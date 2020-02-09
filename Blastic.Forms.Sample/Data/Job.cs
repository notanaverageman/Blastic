using System;
using Blastic.Reactive;

namespace Blastic.Forms.Sample.Data
{
	public class Job
	{
		public int Id { get; set; }
		public int MachineId { get; }

		public IReactiveProperty<string> Name { get; }
		public IReactiveProperty<bool> IsStarted { get; }

		public IReactiveProperty<DateTime> QueueDate { get; }
		public IReactiveProperty<DateTime> StartDate { get; }

		public IReactiveProperty<int> StartFrame { get; }
		public IReactiveProperty<int> EndFrame { get; }

		public Job(int machineId) : this(-1, machineId)
		{
		}

		public Job(int id, int machineId)
		{
			Id = id;
			MachineId = machineId;

			Name = new ReactiveProperty<string>();
			IsStarted = new ReactiveProperty<bool>();

			QueueDate = new ReactiveProperty<DateTime>();
			StartDate = new ReactiveProperty<DateTime>();

			StartFrame = new ReactiveProperty<int>();
			EndFrame = new ReactiveProperty<int>();
		}
	}
}