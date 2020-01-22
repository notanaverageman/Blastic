using Blastic.Reactive;

namespace Blastic.Forms.Sample.UserInterface
{
	public class HomepageViewModel
	{
		public IReactiveProperty<string> Title { get; }

		public HomepageViewModel()
		{
			Title = new ReactiveProperty<string>("Homepage");
		}
	}
}