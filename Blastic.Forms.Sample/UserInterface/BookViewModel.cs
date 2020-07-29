using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Blastic.Platform;
using Blastic.Reactive;
using DynamicData;
using DynamicData.Binding;

namespace Blastic.Forms.Sample.UserInterface
{
	public class BookViewModel
	{
		private readonly ReadOnlyObservableCollection<AuthorViewModel> _authors;

		public int Id { get; }

		public IReactiveProperty<string> Title { get; }
		public IReactiveProperty<string> Description { get; }
		public IReactiveProperty<string> ImageUrl { get; }

		public IReactiveProperty<string> Language { get; }
		public IReactiveProperty<TimeSpan> TotalDuration { get; }

		public IReadOnlyReactiveProperty<string> AuthorNames { get; }

		public ReadOnlyObservableCollection<AuthorViewModel> Authors => _authors;

		public BookViewModel(int id, IEnumerable<AuthorViewModel> authors)
		{
			Id = id;

			Title = new ReactiveProperty<string>();
			Description = new ReactiveProperty<string>();
			ImageUrl = new ReactiveProperty<string>();

			Language = new ReactiveProperty<string>();
			TotalDuration = new ReactiveProperty<TimeSpan>();

			SourceCache<AuthorViewModel, int> authorsCache = new SourceCache<AuthorViewModel, int>(x => x.Id);
			IObservable<IChangeSet<AuthorViewModel, int>> authorsObservable = authorsCache.Connect();

			AuthorNames = authorsObservable
				.DeferUntilLoaded()
				.MergeMany(x => x.FullName)
				.Scan("", (x, y) => string.IsNullOrWhiteSpace(x)
					? y
					: string.IsNullOrWhiteSpace(y)
						? x
						: $"{x}, {y}")
				.ToReadOnlyReactiveProperty();

			authorsObservable
				.SubscribeOn(TaskPoolScheduler.Default)
				.Sort(SortExpressionComparer<AuthorViewModel>.Ascending(x => x.FirstName.Value))
				.ObserveOnUI()
				.Bind(out _authors)
				.DisposeMany()
				.Subscribe();

			authorsCache.AddOrUpdate(authors);
		}
	}
}