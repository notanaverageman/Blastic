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
using Blastic.Forms.Sample.UserInterface.MediaPlayer;
using Blastic.Forms.Services.Navigation;
using Blastic.Forms.UserInterface;
using Blastic.LifetimeManagement;
using Blastic.Ordering;
using Blastic.Platform;
using Blastic.Reactive;
using DynamicData;

namespace Blastic.Forms.Sample.UserInterface
{
	public class HomeViewModel : Screen, IShellTab
	{
		private readonly DownloadService _downloadService;
		private readonly ArchiveOrgService _archiveOrgService;
		private readonly INavigationService _navigationService;
		private readonly ProgramDatabase _database;

		private readonly SourceCache<BookViewModel, string> _booksSource;
		private readonly ReadOnlyObservableCollection<BookViewModel> _books;

		public Order Order { get; }
		public IReadOnlyReactiveProperty<string> Title { get; }
		public IReadOnlyReactiveProperty<string> IconGlyph { get; }

		public MediaPlayerViewModel MediaPlayer { get; }
		public LocalizableProperties LocalizableProperties { get; }

		public ReadOnlyObservableCollection<BookViewModel> Books => _books;

		public IReactiveProperty<BookViewModel> SelectedBook { get; }

		public Command FetchBooksCommand { get; }
		public Command<BookViewModel> ChangeSelectedBook { get; }

		public HomeViewModel(
			MediaPlayerViewModel mediaPlayer,
			DownloadService downloadService,
			ArchiveOrgService archiveOrgService,
			INavigationService navigationService,
			ProgramDatabase database,
			LocalizableProperties localizableProperties)
		{
			_downloadService = downloadService;
			_archiveOrgService = archiveOrgService;
			_navigationService = navigationService;
			_database = database;

			MediaPlayer = mediaPlayer;
			LocalizableProperties = localizableProperties;

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

			SelectedBook = new ReactiveProperty<BookViewModel>();

			FetchBooksCommand = new Command(FetchBooks);
			ChangeSelectedBook = new Command<BookViewModel>(SelectBook);

			Lifetime.Initialization.Subscribe(FetchBooks);
		}

		private async Task SelectBook(BookViewModel book)
		{
			await _navigationService.NavigateTo(this, book);
		}

		private async Task FetchBooks()
		{
			async Task Fetch(CancellationToken cancellationToken)
			{
				ArchiveOrgQueryResult? bookList = await _archiveOrgService.GetAudioBookList(cancellationToken: cancellationToken);

				List<Book> books = bookList.ToBooks();
				List<BookViewModel> viewModels = new List<BookViewModel>();

				foreach (Book book in books)
				{
					BookViewModel viewModel = new BookViewModel(
						book,
						MediaPlayer,
						LocalizableProperties,
						_downloadService,
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