using System.ComponentModel;

namespace Blastic.Reactive
{
	internal class Singletons
	{
		public static readonly PropertyChangedEventArgs PropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(IReactiveProperty.Value));
		public static readonly DataErrorsChangedEventArgs DataErrorsChangedEventArgs = new DataErrorsChangedEventArgs(nameof(IReactiveProperty.Value));
	}
}