using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Forms.Sample.Data;
using Blastic.Forms.Sample.UserInterface.Chapters;
using Blastic.Forms.Sample.UserInterface.Downloads;
using Blastic.Forms.Sample.UserInterface.MediaPlayer;
using Blastic.Forms.Sample.UserInterface.Notifications;
using Blastic.Forms.UserInterface;
using Blastic.LifetimeManagement;
using Blastic.Ordering;
using DynamicData;
using Microsoft.Extensions.Logging;
using ExecutionContext = Blastic.Execution.ExecutionContext;

namespace Blastic.Forms.Sample.UserInterface.Main
{
	public class MainViewModel : ConductorOneActive<IShellTab>
	{
		private readonly ProgramDatabase _programDatabase;
		private readonly ILogger<MainViewModel> _logger;

		public ExecutionContext ExecutionContext { get; }
		
		public MediaPlayerViewModel MediaPlayer { get; }
		public ChapterDetailsViewModel ChapterDetails { get; }
		public DownloadsViewModel Downloads { get; }
		public NotificationsViewModel Notifications { get; }

		public MainViewModel(
			ProgramDatabase programDatabase,
			ILogger<MainViewModel> logger,
			MediaPlayerViewModel mediaPlayer,
			ChapterDetailsViewModel chapterDetails,
			DownloadsViewModel downloads,
			NotificationsViewModel notifications,
			IEnumerable<IShellTab> tabs)
		{
			_programDatabase = programDatabase;
			_logger = logger;
			
			MediaPlayer = mediaPlayer;
			ChapterDetails = chapterDetails;
			Downloads = downloads;
			Notifications = notifications;

			ExecutionContext = new ExecutionContext();

			tabs = tabs
				.OrderBy(x => x.Order)
				.ToList();

			ItemsSource.AddRange(tabs);

			Lifetime.Initialization.Subscribe(
				async x =>
				{
					await MigrateDatabase(x);
				},
				// This order ensures that we are running before child initializations.
				new Order(int.MinValue));

			Lifetime.Activation.Subscribe(async x =>
			{
				await Activate(Items.FirstOrDefault(), x);
			});
		}

		private async Task MigrateDatabase(CancellationToken cancellationToken)
		{
			if (!await _programDatabase.IsMigrationAvailable(cancellationToken))
			{
				return;
			}

			_logger.LogDebug("Checking and applying migrations.");
			await _programDatabase.Migrate(cancellationToken);
			_logger.LogDebug("Finished checking and applying migrations.");
		}
	}
}