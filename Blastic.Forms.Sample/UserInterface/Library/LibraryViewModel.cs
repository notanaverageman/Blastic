using Blastic.Forms.Sample.Icons;
using Blastic.Forms.Sample.Resources;
using Blastic.Forms.UserInterface;
using Blastic.LifetimeManagement;
using Blastic.Ordering;
using Blastic.Reactive;

namespace Blastic.Forms.Sample.UserInterface.Library
{
	public class LibraryViewModel : IShellTab
	{
		public ILifetime Lifetime { get; }
		
		public Order Order { get; }
		public IReadOnlyReactiveProperty<string> Title { get; }
		public IReadOnlyReactiveProperty<string> IconGlyph { get; }

		public LibraryViewModel(LocalizableProperties localizableProperties)
		{
			Lifetime = new Lifetime();
			
			Order = new Order(2);
			Title = localizableProperties.Library.Title;
			IconGlyph = new ReactiveProperty<string>(IconFont.LibraryBooks);
		}
	}
}