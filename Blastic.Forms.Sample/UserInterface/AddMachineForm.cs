using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Blastic.Commanding;
using Blastic.DynamicControls;
using Blastic.DynamicControls.Properties;
using Blastic.Forms.Sample.Localization;
using Blastic.Reactive;

namespace Blastic.Forms.Sample.UserInterface
{
	public class AddMachineForm
	{
		public IReactiveProperty<string> MachineName { get; }
		public IReactiveProperty<int> SecondsPerFrame { get; }

		public DynamicModel Form { get; }

		public AddMachineForm(
			Labels labels,
			IEnumerable<MachineViewModel> machines,
			Func<Task> goBackFunction)
		{
			MachineName = new ReactiveProperty<string>();
			SecondsPerFrame = new ReactiveProperty<int>();

			MachineName.AddValidator(
				x =>
				{
					if (machines.Any(y => y.Machine.Name.Value.Equals(x, StringComparison.InvariantCultureIgnoreCase)))
					{
						return labels.AddMachine.AlreadyExists.Value;
					}

					return null;
				});

			Form = new DynamicModel();

			AsyncCommand okCommand = new AsyncCommand(
				MachineName.HasErrorObservable.Select(x => !x),
				async () =>
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
				.AddText(MachineName, x => x
					.WithLabel(labels.AddMachine.MachineName))
				.AddNumber(SecondsPerFrame, x => x
					.WithLabel(labels.AddMachine.SecondsPerFrame))
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