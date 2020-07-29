using System.Reactive.Linq;
using Blastic.Reactive;

namespace Blastic.Forms.Sample.UserInterface
{
	public class AuthorViewModel
	{
		public int Id { get; }

		public IReactiveProperty<string> FirstName { get; }
		public IReactiveProperty<string> LastName { get; }

		public IReadOnlyReactiveProperty<string> FullName { get; }

		public IReactiveProperty<string> DateOfBirth { get; }
		public IReactiveProperty<string> DateOfDeath { get; }

		public AuthorViewModel(int id)
		{
			Id = id;

			FirstName = new ReactiveProperty<string>();
			LastName = new ReactiveProperty<string>();

			DateOfBirth = new ReactiveProperty<string>();
			DateOfDeath = new ReactiveProperty<string>();

			FullName = FirstName
				.CombineLatest(LastName, (x, y) => x + " " + y)
				.ToReadOnlyReactiveProperty();
		}
	}
}