using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Commanding;
using Blastic.Forms.Sample.ArchiveOrg;
using Blastic.Forms.Sample.Data;
using Blastic.Forms.Sample.Resources;
using Blastic.Forms.Sample.Services;
using Blastic.Forms.Sample.UserInterface.Chapters;
using Blastic.Forms.Sample.UserInterface.Downloads;
using Blastic.Forms.Sample.UserInterface.MediaPlayer;
using Blastic.LifetimeManagement;
using Blastic.Reactive;
using ExecutionContext = Blastic.Execution.ExecutionContext;

namespace Blastic.Forms.Sample.UserInterface.Books
{
	public class BookViewModel : IHasLifetime
	{
		private readonly MediaPlayerViewModel _mediaPlayer;
		private readonly DownloadsViewModel _downloads;
		private readonly ArchiveOrgService _archiveOrgService;
		private readonly ProgramDatabase _database;

		public Book Book { get; }
		public LocalizableProperties LocalizableProperties { get; }

		public ILifetime Lifetime { get; }
		public ExecutionContext ExecutionContext { get; }

		public IReactiveProperty<string> Title { get; }
		public IReactiveProperty<string> Author { get; }
		public IReactiveProperty<string> Description { get; }

		public IReactiveProperty<string> ImageUrl { get; }
		public IReactiveProperty<TimeSpan> TotalDuration { get; }

		public IReactiveProperty<bool> DescriptionExpanded { get; }
		public IReactiveProperty<IReadOnlyReactiveProperty<string>> DescriptionToggleLabel { get; }

		public ObservableCollection<ChapterViewModel> Chapters { get; }
		
		public Command PlayCommand { get; }
		public Command DownloadCommand { get; }
		public Command ToggleDescriptionLengthCommand { get; }

		public Command<ChapterViewModel> ShowDetailsCommand { get; }

		public BookViewModel(
			Book book,
			MediaPlayerViewModel mediaPlayer,
			DownloadsViewModel downloads,
			ChapterDetailsViewModel chapterDetails,
			LocalizableProperties localizableProperties,
			ArchiveOrgService archiveOrgService,
			ProgramDatabase database)
		{
			_mediaPlayer = mediaPlayer;
			_downloads = downloads;
			_archiveOrgService = archiveOrgService;
			_database = database;

			Book = book;
			LocalizableProperties = localizableProperties;

			Lifetime = new Lifetime();
			ExecutionContext = new ExecutionContext();

			Title = new ReactiveProperty<string>(Book.Title);
			Description = new ReactiveProperty<string>(Book.Description);
			Author = new ReactiveProperty<string>(Book.Author);

			string imageUrl = ArchiveOrgService.ArchiveOrgImageUrlPrefix + "/" + Book.ArchiveOrgId;

			ImageUrl = new ReactiveProperty<string>(imageUrl);
			TotalDuration = new ReactiveProperty<TimeSpan>();

			Chapters = new ObservableCollection<ChapterViewModel>();

			DescriptionExpanded = new ReactiveProperty<bool>();
			DescriptionToggleLabel = new ReactiveProperty<IReadOnlyReactiveProperty<string>>(LocalizableProperties.HomeBookDescriptionMore);
			
			PlayCommand = new Command(Play);
			DownloadCommand = new Command(Download);
			ToggleDescriptionLengthCommand = new Command(ToggleDescriptionLength);

			ShowDetailsCommand = new Command<ChapterViewModel>(chapterDetails.Show);

			Lifetime.Initialization.Subscribe(FetchDetails);
		}

		private void Play()
		{
			ChapterViewModel? firstChapter = Chapters.FirstOrDefault();

			if (firstChapter == null)
			{
				return;
			}

			_mediaPlayer.PlayChapter(firstChapter);
		}

		private async Task Download()
		{
			foreach (ChapterViewModel chapter in Chapters)
			{
				await chapter.DownloadCommand.Execute();
			}
		}

		private void ToggleDescriptionLength()
		{
			DescriptionExpanded.Value = !DescriptionExpanded.Value;

			DescriptionToggleLabel.Value = DescriptionExpanded.Value
				? LocalizableProperties.HomeBookDescriptionLess
				: LocalizableProperties.HomeBookDescriptionMore;
		}

		private async Task FetchDetails(CancellationToken cancellationToken)
		{
			if (Chapters.Any())
			{
				return;
			}

			await ExecutionContext.Execute(FetchFromDatabase);

			if (Chapters.Any())
			{
				return;
			}

			await ExecutionContext.Execute(FetchFromArchiveOrg);
		}

		private async Task FetchFromDatabase(CancellationToken cancellationToken)
		{
			Book? book = await _database.BooksTable.Get(Book.ArchiveOrgId, cancellationToken);

			if (book == null)
			{
				return;
			}

			foreach (Chapter chapter in book.Chapters)
			{
				Chapters.Add(new ChapterViewModel(_downloads, _mediaPlayer, this, chapter));
			}
		}

		private async Task FetchFromArchiveOrg(CancellationToken cancellationToken)
		{
			ArchiveOrgMetadata? metadata = await _archiveOrgService.GetAudioBookMetadata(
				Book.ArchiveOrgId,
				cancellationToken);

			if (metadata == null)
			{
				// TODO: Error handling.
				return;
			}

			foreach (ArchiveOrgChapterMetadata chapterMetadata in metadata.Chapters)
			{
				Chapter chapter = new()
				{
					Title = chapterMetadata.Title,
					Duration = chapterMetadata.Duration,
					Order = chapterMetadata.Track,
					FileName = chapterMetadata.FileName,
					SizeInBytes = chapterMetadata.SizeInBytes
				};

				Book.Chapters.Add(chapter);
				Chapters.Add(new ChapterViewModel(_downloads, _mediaPlayer, this, chapter));
			}

			await _database.BooksTable.Put(Book, cancellationToken);
		}
	}
}