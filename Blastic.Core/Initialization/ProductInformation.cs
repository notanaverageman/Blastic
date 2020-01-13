using Blastic.Reactive;

namespace Blastic.Initialization
{
	public class ProductInformation
	{
		public IReactiveProperty<string> ProgramName { get; }
		public IReactiveProperty<System.Version> Version { get; }

		public ProductInformation()
		{
			ProgramName = new ReactiveProperty<string>();
			Version = new ReactiveProperty<System.Version>();
		}
	}
}