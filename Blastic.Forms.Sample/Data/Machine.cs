using Blastic.Reactive;

namespace Blastic.Forms.Sample.Data
{
	public class Machine
	{
		public int Id { get; set; }

		public IReactiveProperty<string> Name { get; }
		public IReactiveProperty<int> SecondsPerFrame { get; }

		public ReactiveCollection<Job> Jobs { get; }

		public Machine()
		{
			Id = -1;

			Name = new ReactiveProperty<string>();
			SecondsPerFrame = new ReactiveProperty<int>();

			Jobs = new ReactiveCollection<Job>();
		}
	}
}