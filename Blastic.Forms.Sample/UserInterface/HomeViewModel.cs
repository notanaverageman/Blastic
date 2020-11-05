using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Commanding;
using Blastic.Forms.Sample.ControlExtensions;
using Blastic.Forms.Sample.Librivox;
using Blastic.Forms.Sample.Localization;
using Blastic.Forms.Services.Navigation;
using Blastic.Forms.UserInterface;
using Blastic.LifetimeManagement;
using Blastic.Ordering;
using Blastic.Reactive;
using DynamicData;

namespace Blastic.Forms.Sample.UserInterface
{
	public class HomeViewModel : Screen, IShellTab
	{
		private readonly INavigationService _navigationService;
		private readonly HttpClient _httpClient;

		public IReactiveProperty<PanState> PanState { get; }

		private readonly SourceCache<BookViewModel, int> _booksSource;
		private readonly ReadOnlyObservableCollection<BookViewModel> _books;

		public Order Order { get; }
		public IReadOnlyReactiveProperty<string> Title { get; }

		public ReadOnlyObservableCollection<BookViewModel> Books => _books;
		public IReactiveProperty<BookViewModel> SelectedBook { get; }

		public AsyncCommand FetchBooksCommand { get; }
		public Command<PanState> TogglePanStateCommand { get; }

		public HomeViewModel(
			INavigationService navigationService,
			HttpClient httpClient,
			Labels labels)
		{
			_navigationService = navigationService;
			_httpClient = httpClient;

			Order = new Order(0);
			Title = labels.Home.Title;

			_booksSource = new SourceCache<BookViewModel, int>(x => x.Id);
			_booksSource
				.Connect()
				.Bind(out _books)
				.DisposeMany()
				.Subscribe();

			SelectedBook = new ReactiveProperty<BookViewModel>();

			FetchBooksCommand = new AsyncCommand(FetchBooks);

			PanState = new ReactiveProperty<PanState>();

			TogglePanStateCommand = new Command<PanState>(
				x =>
				{
					PanState.Value = x.Parameter;
				});

			AuthorViewModel author = new AuthorViewModel(123);
			author.FirstName.Value = "Jerry";
			author.LastName.Value = "Bonnell";

			for (int i = 0; i < 1; i++)
			{
				BookViewModel book = new BookViewModel(i, new[] { author });
				book.Title.Value = "Golden mean to 5000 digits";
				book.ImageUrl.Value = "https://ia800301.us.archive.org/21/items/golden_mean_to_5000_digits_0810_librivox/Golden_mean_5000_digits_1201.jpg?cnt=0";

				_booksSource.AddOrUpdate(book);
			}

			SelectedBook.Value = _books.FirstOrDefault();

			Lifetime.Initialize.Subscribe(
				async x =>
				{
					await Task.Delay(TimeSpan.FromSeconds(1));
					PanState.Value = ControlExtensions.PanState.Collapsed;
				});
		}

		private async Task FetchBooks()
		{
			async Task Fetch(CancellationToken cancellationToken)
			{
				HttpResponseMessage responseMessage = await _httpClient.GetAsync(
					"https://librivox.org/api/feed/audiobooks?format=json&extended=true",
					cancellationToken);

				string result = await responseMessage.Content.ReadAsStringAsync();

				LibrivoxBookList bookList = JsonSerializer.Deserialize<LibrivoxBookList>(result);
				List<BookViewModel> bookViewModels = bookList.ToViewModels();

				_booksSource.Edit(
					x =>
					{
						x.Clear();
						x.AddOrUpdate(bookViewModels);
					});

				SelectedBook.Value = _books.FirstOrDefault();
			}

			await ExecutionContext.Execute(Fetch);
		}
	}
}