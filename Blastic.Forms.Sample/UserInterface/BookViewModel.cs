using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Commanding;
using Blastic.Forms.Sample.Controls.Overlay;
using Blastic.Forms.Sample.Data;
using Blastic.Forms.Sample.Librivox;
using Blastic.Forms.Sample.Services;
using Blastic.Forms.Sample.UserInterface.MediaPlayer;
using Blastic.LifetimeManagement;
using Blastic.Reactive;

namespace Blastic.Forms.Sample.UserInterface
{
	public class BookViewModel : Screen
	{
		private readonly MediaPlayerViewModel _mediaPlayer;
		private readonly DownloadService _downloadService;
		private readonly ArchiveOrgService _archiveOrgService;
		private readonly ProgramDatabase _database;

		public Book Book { get; }

		public IReactiveProperty<string> Title { get; }
		public IReactiveProperty<string> Author { get; }
		public IReactiveProperty<string> Description { get; }

		public IReactiveProperty<string> ImageUrl { get; }
		public IReactiveProperty<TimeSpan> TotalDuration { get; }

		public IReactiveProperty<int> DescriptionMaxLines { get; }
		public IReactiveProperty<string> ToggleDescriptionLabel { get; }

		public ObservableCollection<ChapterViewModel> Chapters { get; }

		public IReactiveProperty<OverlayState> ChapterDetailsOverlayState { get; }
		public IReactiveProperty<ChapterViewModel> ChapterForDetails { get; }

		public Command ToggleDescriptionLengthCommand { get; }
		public Command<ChapterViewModel> PlayChapterCommand { get; }

		public Command<ChapterViewModel> ShowDetailsCommand { get; }
		public Command<ChapterViewModel> HideDetailsCommand { get; }

		public BookViewModel(
			Book book,
			MediaPlayerViewModel mediaPlayer,
			DownloadService downloadService,
			ArchiveOrgService archiveOrgService,
			ProgramDatabase database)
		{
			_mediaPlayer = mediaPlayer;
			_downloadService = downloadService;
			_archiveOrgService = archiveOrgService;
			_database = database;

			Book = book;

			Title = new ReactiveProperty<string>(Book.Title);
			Description = new ReactiveProperty<string>(Book.Description);
			Author = new ReactiveProperty<string>(Book.Author);

			string imageUrl = ArchiveOrgService.ArchiveOrgImageUrlPrefix + "/" + Book.ArchiveOrgId;

			ImageUrl = new ReactiveProperty<string>(imageUrl);
			TotalDuration = new ReactiveProperty<TimeSpan>();

			Chapters = new ObservableCollection<ChapterViewModel>();

			DescriptionMaxLines = new ReactiveProperty<int>(3);
			ToggleDescriptionLabel = new ReactiveProperty<string>("More");

			ChapterDetailsOverlayState = new ReactiveProperty<OverlayState>();
			ChapterForDetails = new ReactiveProperty<ChapterViewModel>();

			ToggleDescriptionLengthCommand = new Command(ToggleDescriptionLength);
			PlayChapterCommand = new Command<ChapterViewModel>(mediaPlayer.PlayChapter);

			ShowDetailsCommand = new Command<ChapterViewModel>(ShowDetails);
			HideDetailsCommand = new Command<ChapterViewModel>(HideDetails);

			Lifetime.Initialization.Subscribe(FetchDetails);
		}

		private void ToggleDescriptionLength()
		{
			if (ToggleDescriptionLabel.Value == "More")
			{
				ToggleDescriptionLabel.Value = "Less";
				DescriptionMaxLines.Value = -1;
			}
			else
			{
				ToggleDescriptionLabel.Value = "More";
				DescriptionMaxLines.Value = 3;
			}
		}

		private void ShowDetails(ChapterViewModel chapter)
		{
			ChapterForDetails.Value = chapter;
			ChapterDetailsOverlayState.Value = OverlayState.Expanded;
		}

		private void HideDetails()
		{
			ChapterDetailsOverlayState.Value = OverlayState.Invisible;
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
				Chapters.Add(new ChapterViewModel(_downloadService, _mediaPlayer, this, chapter));
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
				Chapter chapter = new Chapter
				{
					Title = chapterMetadata.Title,
					Duration = chapterMetadata.Duration,
					Order = chapterMetadata.Track,
					FileName = chapterMetadata.FileName,
					SizeInBytes = chapterMetadata.SizeInBytes
				};

				Book.Chapters.Add(chapter);
				Chapters.Add(new ChapterViewModel(_downloadService, _mediaPlayer, this, chapter));
			}

			await _database.BooksTable.Put(Book, cancellationToken);
		}
	}
}