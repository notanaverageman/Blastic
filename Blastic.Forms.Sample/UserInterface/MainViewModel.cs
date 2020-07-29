using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Forms.UserInterface;
using Blastic.Initialization.Steps;
using Blastic.LifetimeManagement;
using Blastic.Ordering;

namespace Blastic.Forms.Sample.UserInterface
{
	public class MainViewModel : ConductorOneActive<IShellTab>
	{
		public MainViewModel(
			IEnumerable<IShellTab> tabs,
			IEnumerable<IInitializationStep> initializationSteps)
		{
			tabs = tabs
				.OrderBy(x => x.Order)
				.ToList();

			Items.AddRange(tabs);

			Lifetime.Initialize.Subscribe(
				async x =>
				{
					await ExecuteInitializationSteps(x.CancellationToken, initializationSteps);
				},
				// This order ensures that we are running before child initializations.
				new Order(int.MinValue));

			Lifetime.Activate.Subscribe(async x =>
			{
				await Activate(Items.FirstOrDefault(), x.CancellationToken);
			});
		}

		private async Task ExecuteInitializationSteps(
			CancellationToken cancellationToken,
			IEnumerable<IInitializationStep> initializationSteps)
		{
			foreach (IInitializationStep initializationStep in initializationSteps)
			{
				if (!await initializationStep.ShouldExecute(cancellationToken))
				{
					continue;
				}

				await ExecutionContext.Execute(
					initializationStep.Execute,
					initializationStep.Description,
					initializationStep.ShowBusyIndicator,
					rethrowUnhandledException: false,
					initializationStep.IsCancellationSupported,
					cancellationToken);
			}
		}
	}
}