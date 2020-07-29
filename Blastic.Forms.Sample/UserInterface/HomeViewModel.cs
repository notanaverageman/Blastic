using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Commanding;
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

		private readonly SourceCache<BookViewModel, int> _booksSource;
		private readonly ReadOnlyObservableCollection<BookViewModel> _books;

		public Order Order { get; }
		public IReadOnlyReactiveProperty<string> Title { get; }

		public ReadOnlyObservableCollection<BookViewModel> Books => _books;

		public AsyncCommand FetchBooksCommand { get; }

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

			FetchBooksCommand = new AsyncCommand(FetchBooks);
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
			}

			await ExecutionContext.Execute(Fetch);
		}
	}
}