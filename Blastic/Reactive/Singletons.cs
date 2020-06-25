using System.ComponentModel;
using System.Reactive.Linq;

namespace Blastic.Reactive
{
	internal class Singletons
	{
		public static readonly PropertyChangedEventArgs PropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(IReactiveProperty.Value));
		public static readonly DataErrorsChangedEventArgs DataErrorsChangedEventArgs = new DataErrorsChangedEventArgs(nameof(IReactiveProperty.Value));
		public static readonly IReadOnlyReactiveProperty<bool> TrueReadOnlyReactiveProperty = Observable
			.Repeat(true, 1)
			.ToReadOnlyReactiveProperty();
	}
}