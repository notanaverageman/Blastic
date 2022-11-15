using System.ComponentModel;

namespace Blastic.Reactive
{
	internal class Singletons
	{
		public static readonly PropertyChangedEventArgs PropertyChangedEventArgs = new(nameof(IReactiveProperty.Value));
		public static readonly DataErrorsChangedEventArgs DataErrorsChangedEventArgs = new(nameof(IReactiveProperty.Value));
		public static readonly IReadOnlyReactiveProperty<bool> TrueReadOnlyReactiveProperty = new ConstantReactiveProperty<bool>(true);
	}
}