using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Commanding;
using Blastic.Forms.Sample.ArchiveOrg;
using Blastic.Forms.Sample.Data;
using Blastic.Forms.Sample.Icons;
using Blastic.Forms.Sample.Resources;
using Blastic.Forms.Sample.Services;
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

namespace Blastic.Forms.Sample.UserInterface
{
	public class SearchViewModel : IViewAware, IShellTab
	{
		private readonly ProgramDatabase _database;
		private readonly DownloadsViewModel _downloads;
		private readonly MediaPlayerViewModel _mediaPlayer;
		private readonly ChapterDetailsViewModel _chapterDetails;
		private readonly ArchiveOrgService _archiveOrgService;
		private readonly INavigationService _navigationService;

		private readonly SourceCache<BookViewModel, string> _booksSource;
		private readonly ReadOnlyObservableCollection<BookViewModel> _books;

		private CancellationTokenSource? _searchCancellationTokenSource;

		public ILifetime Lifetime { get; }
		public ExecutionContext ExecutionContext { get; }
		public IReactiveProperty<object?> View { get; }

		public Order Order { get; }
		public IReadOnlyReactiveProperty<string> Title { get; }
		public IReadOnlyReactiveProperty<string> IconGlyph { get; }

		public LocalizableProperties LocalizableProperties { get; }
		
		public ReadOnlyObservableCollection<BookViewModel> Books => _books;
		public IReactiveProperty<string> SearchQuery { get; }

		public Command<BookViewModel> NavigateToBookCommand { get; }

		public SearchViewModel(
			ProgramDatabase database,
			DownloadsViewModel downloads,
			MediaPlayerViewModel mediaPlayer,
			ChapterDetailsViewModel chapterDetails,
			ArchiveOrgService archiveOrgService,
			INavigationService navigationService,
			LocalizableProperties localizableProperties)
		{
			_database = database;
			_downloads = downloads;
			_mediaPlayer = mediaPlayer;
			_chapterDetails = chapterDetails;
			_archiveOrgService = archiveOrgService;
			_navigationService = navigationService;
			LocalizableProperties = localizableProperties;

			Lifetime = new Lifetime();
			ExecutionContext = new ExecutionContext();
			View = new ReactiveProperty<object?>();

			Order = new Order(1);
			Title = localizableProperties.SearchTitle;
			IconGlyph = new ReactiveProperty<string>(IconFont.Magnify);
			
			_booksSource = new SourceCache<BookViewModel, string>(x => x.Book.ArchiveOrgId);
			_booksSource
				.Connect()
				.ObserveOnUI()
				.Bind(out _books)
				.DisposeMany()
				.Subscribe();

			SearchQuery = new ReactiveProperty<string>();
			SearchQuery
				.Throttle(TimeSpan.FromMilliseconds(100), Scheduler.Default)
				.Select(x => x?.Trim())
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.DistinctUntilChanged()
				.Subscribe(Search);

			NavigateToBookCommand = new Command<BookViewModel>(NavigateToBook);
		}

		private async void Search(string query)
		{
			async Task SearchAsync(CancellationToken cancellationToken)
			{
				ArchiveOrgQueryResult result = await _archiveOrgService.Search(
					query,
					cancellationToken: cancellationToken);

				List<Book> books = result.ToBooks();

				_booksSource.Edit(
					x =>
					{
						x.Clear();
						
						foreach (Book book in books)
						{
							BookViewModel viewModel = new(
								book,
								_mediaPlayer,
								_downloads,
								_chapterDetails,
								LocalizableProperties,
								_archiveOrgService,
								_database);

							x.AddOrUpdate(viewModel);
						}
					});
			}

			_searchCancellationTokenSource?.Cancel();
			_searchCancellationTokenSource?.Dispose();

			_searchCancellationTokenSource = new CancellationTokenSource();

			await ExecutionContext.Execute(
				SearchAsync,
				customCancellationToken: _searchCancellationTokenSource.Token);
		}

		private async Task NavigateToBook(BookViewModel book)
		{
			await _navigationService.NavigateTo(this, book);
		}
	}
}