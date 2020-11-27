using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Commanding;
using Blastic.Forms.Sample.Controls.Overlay;
using Blastic.Forms.Sample.Data;
using Blastic.Forms.Sample.Icons;
using Blastic.Forms.Sample.Librivox;
using Blastic.Forms.Sample.Localization;
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
		private readonly HttpClient _httpClient;
		private readonly ProgramDatabase _database;

		public IReactiveProperty<OverlayState> OverlayState { get; }

		private readonly SourceCache<BookViewModel, string> _booksSource;
		private readonly ReadOnlyObservableCollection<BookViewModel> _books;

		public Order Order { get; }
		public IReadOnlyReactiveProperty<string> Title { get; }
		public IReadOnlyReactiveProperty<string> IconGlyph { get; }

		public ReadOnlyObservableCollection<BookViewModel> Books => _books;

		public IReactiveProperty<BookViewModel> SelectedBook { get; }

		public Command FetchBooksCommand { get; }
		public Command<OverlayState> ToggleOverlayStateCommand { get; }

		public HomeViewModel(
			HttpClient httpClient,
			ProgramDatabase database,
			Labels labels)
		{
			_httpClient = httpClient;
			_database = database;

			Order = new Order(0);
			Title = labels.Home.Title;
			IconGlyph = new ReactiveProperty<string>(IconFont.Home);

			_booksSource = new SourceCache<BookViewModel, string>(x => x.Id);

			_booksSource
				.Connect()
				.ObserveOnUI()
				.Bind(out _books)
				.DisposeMany()
				.Subscribe();

			OverlayState = new ReactiveProperty<OverlayState>();
			SelectedBook = new ReactiveProperty<BookViewModel>();

			FetchBooksCommand = new Command(FetchBooks);

			ToggleOverlayStateCommand = new Command<OverlayState>(
				x =>
				{
					OverlayState.Value = x;
				});

			Lifetime.Initialization.Subscribe(
				() =>
				{
					OverlayState.Value = Controls.Overlay.OverlayState.Collapsed;
				});

			Lifetime.Initialization.Subscribe(FetchBooks);

			_booksSource.AddOrUpdate(new BookViewModel("adventures_lightfoot_the_deer_js_1804_librivox")
			{
				Creator = { Value = "Thornton W. Burgess" },
				Title = { Value = "The Adventures of Lightfoot the Deer (Version 2)" },
				ImageUrl = { Value = "https://archive.org/services/img/adventures_lightfoot_the_deer_js_1804_librivox" }
			});
		}

		private async Task FetchBooks()
		{
			async Task Fetch(CancellationToken cancellationToken)
			{
				string url = "https://archive.org/advancedsearch.php?q=collection:librivoxaudio";

				url += @"&fl[]=identifier";
				url += @"&fl[]=title";
				url += @"&fl[]=creator";
				url += @"&fl[]=date";
				url += @"&fl[]=downloads";
				url += @"&rows=50";
				url += @"&page=1";
				url += @"&output=json";

				HttpResponseMessage responseMessage = await _httpClient.GetAsync(url, cancellationToken);

				string result = await responseMessage.Content.ReadAsStringAsync();

				ArchiveOrgQueryResult bookList = JsonSerializer.Deserialize<ArchiveOrgQueryResult>(
					result,
					new JsonSerializerOptions
					{
						PropertyNameCaseInsensitive = true
					});

				List<BookViewModel> bookViewModels = bookList.ToViewModels();

				_booksSource.Edit(
					x =>
					{
						x.Clear();
						x.AddOrUpdate(bookViewModels);
					});

				List<Book> books = new List<Book>();
				foreach (BookViewModel bookViewModel in bookViewModels)
				{
					Book book = new Book
					{
						ArchiveOrgId = bookViewModel.Id,
						Title = bookViewModel.Title.Value,
						Description = bookViewModel.Description.Value
					};

					books.Add(book);
				}

				await _database.BooksTable.PutAll(books, cancellationToken);
			}

			await ExecutionContext.Execute(
				cancellationToken => Task.Run(() => Fetch(cancellationToken), cancellationToken),
				rethrowUnhandledException: true);
		}
	}
}