using Blastic.Reactive;

namespace Blastic.DynamicControls.Elements
{
	public class TextField : Field
	{
		public IReactiveProperty<string?> Mask { get; set; }
        public IReactiveProperty<Keyboard> Keyboard { get; set; }

		public TextField(IReactiveProperty property) : base(property)
		{
			Mask = new ReactiveProperty<string?>(default);
            Keyboard = new ReactiveProperty<Keyboard>(Elements.Keyboard.Default);
		}
	}
}