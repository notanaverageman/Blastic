using Blastic.Reactive;

namespace Blastic.Controls.DynamicControls.Elements.Text
{
	public class TextField : Field
	{
		public IReactiveProperty<string> Mask { get; set; }

		public TextField(IReactiveProperty property) : base(property)
		{
			Mask = new ReactiveProperty<string>();
		}

		protected override Presenter CreatePresenter()
		{
			return new TextPresenter
			{
				Mask = Mask
			};
		}
	}
}