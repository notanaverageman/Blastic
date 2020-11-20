using Blastic.Forms.Sample.Icons;
using Blastic.Forms.Sample.Localization;
using Blastic.Forms.UserInterface;
using Blastic.LifetimeManagement;
using Blastic.Ordering;
using Blastic.Reactive;

namespace Blastic.Forms.Sample.UserInterface
{
	public class SearchViewModel : Screen, IShellTab
	{
		public Order Order { get; }
		public IReadOnlyReactiveProperty<string> Title { get; }
		public IReadOnlyReactiveProperty<string> IconGlyph { get; }

		public SearchViewModel(Labels labels)
		{
			Order = new Order(1);
			Title = labels.Search.Title;
			IconGlyph = new ReactiveProperty<string>(IconFont.Magnify);
		}
	}
}