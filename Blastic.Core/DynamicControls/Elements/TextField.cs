using Blastic.Reactive;

namespace Blastic.DynamicControls.Elements
{
	public class TextField : Field
	{
		public IReactiveProperty<string> Mask { get; set; }

		public TextField(IReactiveProperty property) : base(property)
		{
			Mask = new ReactiveProperty<string>();
		}
	}
}