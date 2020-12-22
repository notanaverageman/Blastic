using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Commanding;
using Blastic.Forms.Sample.ArchiveOrg;
using Blastic.Forms.Sample.Data;
using Blastic.Forms.Sample.Icons;
using Blastic.Forms.Sample.Resources;
using Blastic.Forms.Sample.Services;
using Blastic.Forms.Sample.UserInterface.Books;
using Blastic.Forms.Sample.UserInterface.Chapters;
using Blastic.Forms.Sample.UserInterface.Downloads;
using Blastic.Forms.Sample.UserInterface.MediaPlayer;
using Blastic.Forms.Services.Navigation;
using Blastic.Forms.UserInterface;
using Blastic.LifetimeManagement;
using Blastic.Ordering;
using Blastic.Platform;
using Blastic.Reactive;
using Blastic.ViewManagement;
using DynamicData;
using ExecutionContext = Blastic.Execution.ExecutionContext;

namespace Blastic.Forms.Sample.UserInterface.Home
{
	public class HomeViewModel : IShellTab, IViewAware
	{
		private readonly DownloadsViewModel _downloads;
		private readonly ChapterDetailsViewModel _chapterDetails;
		private readonly ArchiveOrgService _archiveOrgService;
		private readonly INavigationService _navigationService;
		private readonly ProgramDatabase _database;

		private readonly SourceCache<BookViewModel, string> _booksSource;
		private readonly ReadOnlyObservableCollection<BookViewModel> _books;

		public ILifetime Lifetime { get; }
		public ExecutionContext ExecutionContext { get; }
		public IReactiveProperty<object?> View { get; }

		public Order Order { get; }
		public IReadOnlyReactiveProperty<string> Title { get; }
		public IReadOnlyReactiveProperty<string> IconGlyph { get; }

		public MediaPlayerViewModel MediaPlayer { get; }
		public LocalizableProperties LocalizableProperties { get; }

		public ReadOnlyObservableCollection<BookViewModel> Books => _books;
		
		public Command ShowDownloadsCommand { get; }
		
		public Command FetchBooksCommand { get; }
		public Command<BookViewModel> NavigateToBookCommand { get; }

		public HomeViewModel(
			DownloadsViewModel downloads,
			MediaPlayerViewModel mediaPlayer,
			ChapterDetailsViewModel chapterDetails,
			ArchiveOrgService archiveOrgService,
			INavigationService navigationService,
			ProgramDatabase database,
			LocalizableProperties localizableProperties)
		{
			_downloads = downloads;
			_chapterDetails = chapterDetails;
			_archiveOrgService = archiveOrgService;
			_navigationService = navigationService;
			_database = database;

			MediaPlayer = mediaPlayer;
			LocalizableProperties = localizableProperties;

			Lifetime = new Lifetime();
			ExecutionContext = new ExecutionContext();
			View = new ReactiveProperty<object?>();

			Order = new Order(0);
			Title = localizableProperties.HomeTitle;
			IconGlyph = new ReactiveProperty<string>(IconFont.Home);

			_booksSource = new SourceCache<BookViewModel, string>(x => x.Book.ArchiveOrgId);

			_booksSource
				.Connect()
				.ObserveOnUI()
				.Bind(out _books)
				.DisposeMany()
				.Subscribe();

			ShowDownloadsCommand = new Command(downloads.Show);

			FetchBooksCommand = new Command(FetchBooks);
			NavigateToBookCommand = new Command<BookViewModel>(NavigateToBook);

			Lifetime.Initialization.Subscribe(FetchBooks);
		}

		private async Task NavigateToBook(BookViewModel book)
		{
			await _navigationService.NavigateTo(this, book);
		}

		private async Task FetchBooks()
		{
			async Task Fetch(CancellationToken cancellationToken)
			{
				ArchiveOrgQueryResult bookList = await _archiveOrgService.GetAudioBookList(cancellationToken: cancellationToken);

				List<Book> books = bookList.ToBooks();
				List<BookViewModel> viewModels = new();

				foreach (Book book in books)
				{
					BookViewModel viewModel = new(
						book,
						MediaPlayer,
						_downloads,
						_chapterDetails,
						LocalizableProperties,
						_archiveOrgService,
						_database);
					viewModels.Add(viewModel);
				}

				_booksSource.Edit(
					x =>
					{
						x.Clear();
						x.AddOrUpdate(viewModels);
					});

				await _database.BooksTable.PutAll(books, cancellationToken);
			}

			await ExecutionContext.Execute(
				cancellationToken => Task.Run(() => Fetch(cancellationToken), cancellationToken),
				rethrowUnhandledException: true);
		}
	}
}