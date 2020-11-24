using System;
using Blastic.Reactive;

namespace Blastic.Forms.Sample.UserInterface
{
	public class BookViewModel
	{
		public string Id { get; }

		public IReactiveProperty<string> Title { get; }
		public IReactiveProperty<string> Description { get; }
		public IReactiveProperty<string> ImageUrl { get; }

		public IReactiveProperty<TimeSpan> TotalDuration { get; }

		public IReactiveProperty<string> Creator { get; }

		public BookViewModel(string id)
		{
			Id = id;

			Title = new ReactiveProperty<string>();
			Description = new ReactiveProperty<string>();
			ImageUrl = new ReactiveProperty<string>();

			TotalDuration = new ReactiveProperty<TimeSpan>();

			Creator = new ReactiveProperty<string>();
		}
	}
}