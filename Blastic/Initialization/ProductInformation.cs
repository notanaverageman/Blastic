using System;
using Reactive.Bindings;

namespace Blastic.Initialization
{
	public class ProductInformation
	{
		public IReactiveProperty<string> ProgramName { get; }
		public IReactiveProperty<Version> Version { get; }

		public ProductInformation()
		{
			ProgramName = new ReactiveProperty<string>();
			Version = new ReactiveProperty<Version>();
		}
	}
}