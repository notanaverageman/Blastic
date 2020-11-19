using System;
using Blastic.Reactive;
using Blastic.Services.Localization;

namespace Blastic.Forms.Sample.Localization
{
	public class Labels
	{
		private readonly ILocalizationService _localizationService;

		public IReadOnlyReactiveProperty<string> Create { get; }
		public IReadOnlyReactiveProperty<string> Cancel { get; }

		public HomeLabels Home { get; }
		public SearchLabels Search { get; }
		public LibraryLabels Library { get; }

		public Labels(ILocalizationService localizationService)
		{
			_localizationService = localizationService;

			Create = CreateProperty("Sample.Create");
			Cancel = CreateProperty("Sample.Cancel");

			Home = new HomeLabels(CreateProperty);
			Search = new SearchLabels(CreateProperty);
			Library = new LibraryLabels(CreateProperty);
		}

		private IReadOnlyReactiveProperty<string> CreateProperty(string key)
		{
			return new LocalizableReactiveProperty(_localizationService, key);
		}

		public class HomeLabels
		{
			public IReadOnlyReactiveProperty<string> Title { get; }

			public HomeLabels(Func<string, IReadOnlyReactiveProperty<string>> createProperty)
			{
				Title = createProperty("Sample.Home.Title");
			}
		}

		public class SearchLabels
		{
			public IReadOnlyReactiveProperty<string> Title { get; }

			public SearchLabels(Func<string, IReadOnlyReactiveProperty<string>> createProperty)
			{
				Title = createProperty("Sample.Search.Title");
			}
		}

		public class LibraryLabels
		{
			public IReadOnlyReactiveProperty<string> Title { get; }

			public LibraryLabels(Func<string, IReadOnlyReactiveProperty<string>> createProperty)
			{
				Title = createProperty("Sample.Library.Title");
			}
		}
	}
}