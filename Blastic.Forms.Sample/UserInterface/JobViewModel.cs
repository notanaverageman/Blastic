using Blastic.Forms.Sample.Data;

namespace Blastic.Forms.Sample.UserInterface
{
	public class JobViewModel
	{
		public Job Job { get; }

		public JobViewModel(Job job)
		{
			Job = job;
		}
	}
}