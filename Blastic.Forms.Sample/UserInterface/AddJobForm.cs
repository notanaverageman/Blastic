using System;
using System.Threading.Tasks;
using Blastic.Commanding;
using Blastic.DynamicControls;
using Blastic.DynamicControls.Properties;
using Blastic.Forms.Sample.Localization;
using Blastic.Reactive;

namespace Blastic.Forms.Sample.UserInterface
{
	public class AddJobForm
	{
		public IReactiveProperty<string> SceneName { get; }

		public IReactiveProperty<int> StartFrame { get; }
		public IReactiveProperty<int> EndFrame { get; }

		public DynamicModel Form { get; }

		public AddJobForm(
			Labels labels,
			Func<Task> goBackFunction)
		{
			SceneName = new ReactiveProperty<string>();

			StartFrame = new ReactiveProperty<int>();
			EndFrame = new ReactiveProperty<int>();

			Form = new DynamicModel();

			AsyncCommand okCommand = new AsyncCommand(async () =>
			{
				await goBackFunction();
				Form.Ok();
			});

			AsyncCommand cancelCommand = new AsyncCommand(async () =>
			{
				await goBackFunction();
				Form.Cancel();
			});

			Form
				.AddText(SceneName, x => x
					.WithLabel(labels.AddJob.SceneName))
				.AddNumber(StartFrame, x => x
					.WithLabel(labels.AddJob.StartFrame))
				.AddNumber(EndFrame, x => x
					.WithLabel(labels.AddJob.EndFrame))
				.AddGroup(x => x
					.WithHorizontalAlignment(HorizontalAlignment.Stretch)
					.AddAction(okCommand, y => y
						.WithLabel(labels.Create)
						.WithHorizontalAlignment(HorizontalAlignment.Stretch))
					.AddAction(cancelCommand, y => y
						.WithLabel(labels.Cancel)
						.WithHorizontalAlignment(HorizontalAlignment.Stretch)));
		}
	}
}