using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Forms.Sample.UserInterface.MediaPlayer;
using Blastic.Forms.UserInterface;
using Blastic.Initialization.Steps;
using Blastic.LifetimeManagement;
using Blastic.Ordering;

namespace Blastic.Forms.Sample.UserInterface
{
	public class MainViewModel : ConductorOneActive<IShellTab>
	{
		public MediaPlayerViewModel MediaPlayer { get; }

		public MainViewModel(
			MediaPlayerViewModel mediaPlayer,
			IEnumerable<IShellTab> tabs,
			IEnumerable<IInitializationStep> initializationSteps)
		{
			MediaPlayer = mediaPlayer;

			tabs = tabs
				.OrderBy(x => x.Order)
				.ToList();

			Items.AddRange(tabs);

			Lifetime.Initialization.Subscribe(
				async x =>
				{
					await ExecuteInitializationSteps(x, initializationSteps);
				},
				// This order ensures that we are running before child initializations.
				new Order(int.MinValue));

			Lifetime.Activation.Subscribe(async x =>
			{
				await Activate(Items.FirstOrDefault(), x);
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