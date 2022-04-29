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
using Blastic.Platform;
using Blastic.Reactive;
using DynamicData;
using ExecutionContext = Blastic.Execution.ExecutionContext;

namespace Blastic.Forms.Sample.UserInterface.Books
{
	public class BookViewModel : IHasAsyncLifetime
	{
		private readonly MediaPlayerViewModel _mediaPlayer;
		private readonly DownloadsViewModel _downloads;
		private readonly ArchiveOrgService _archiveOrgService;
		private readonly ProgramDatabase _database;

		private readonly SourceCache<ChapterViewModel, string> _chaptersSource;
		private readonly ReadOnlyObservableCollection<ChapterViewModel> _chapters;

		public Book Book { get; }
		public LocalizableProperties LocalizableProperties { get; }

		public IAsyncLifetime Lifetime { get; }
		public ExecutionContext ExecutionContext { get; }

		public IReactiveProperty<string> Title { get; }
		public IReactiveProperty<string> Author { get; }
		public IReactiveProperty<string> Description { get; }

		public IReactiveProperty<string> ImageUrl { get; }
		public IReactiveProperty<TimeSpan> TotalDuration { get; }

		public IReactiveProperty<bool> DescriptionExpanded { get; }
		public IReactiveProperty<IReadOnlyReactiveProperty<string>> DescriptionToggleLabel { get; }

		public ReadOnlyObservableCollection<ChapterViewModel> Chapters => _chapters;

		public Command PlayCommand { get; }
		public Command ToggleDescriptionLengthCommand { get; }
		public AsyncCommand DownloadCommand { get; }

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

			Lifetime = new AsyncLifetime();
			ExecutionContext = new ExecutionContext();

			Title = new ReactiveProperty<string>(Book.Title);
			Description = new ReactiveProperty<string>(Book.Description);
			Author = new ReactiveProperty<string>(Book.Author);

			string imageUrl = ArchiveOrgService.ArchiveOrgImageUrlPrefix + "/" + Book.ArchiveOrgId;

			ImageUrl = new ReactiveProperty<string>(imageUrl);
			TotalDuration = new ReactiveProperty<TimeSpan>();

			DescriptionExpanded = new ReactiveProperty<bool>();
			DescriptionToggleLabel = new ReactiveProperty<IReadOnlyReactiveProperty<string>>(LocalizableProperties.Home.Book.Description.More);

			_chaptersSource = new SourceCache<ChapterViewModel, string>(x => x.Media.Url.Value);
			_chaptersSource
				.Connect()
				.ObserveOnUI()
				.Bind(out _chapters)
				.DisposeMany()
				.Subscribe();

			DownloadCommand = _chaptersSource
				.Connect()
				.TrueForAny(
					x => x.Download.DownloadCommand.CanExecuteObservable,
					x => x)
				.ToAsyncCommand()
				.WithSubscribe(Download);

			PlayCommand = new Command(Play);
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
				await chapter.Download.DownloadCommand.Execute();
			}
		}

		private void ToggleDescriptionLength()
		{
			DescriptionExpanded.Value = !DescriptionExpanded.Value;

			DescriptionToggleLabel.Value = DescriptionExpanded.Value
				? LocalizableProperties.Home.Book.Description.Less
				: LocalizableProperties.Home.Book.Description.More;
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

		private Task FetchFromDatabase(CancellationToken cancellationToken)
		{
			Book? book = _database.BooksTable.Get(Book.ArchiveOrgId);

			if (book == null)
			{
				return Task.CompletedTask;
			}

			foreach (Chapter chapter in book.Chapters)
			{
				_chaptersSource.AddOrUpdate(new ChapterViewModel(_downloads, _mediaPlayer, this, chapter));
			}

			return Task.CompletedTask;
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
				_chaptersSource.AddOrUpdate(new ChapterViewModel(_downloads, _mediaPlayer, this, chapter));
			}

			_database.BooksTable.Put(Book);
		}
	}
}