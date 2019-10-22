using Reactive.Bindings;

namespace Blastic.Controls.DynamicControls.Elements.Boolean
{
	public class BooleanField : Field
	{
		public BooleanField(IReactiveProperty property) : base(property)
		{
		}

		protected override Presenter CreatePresenter()
		{
			return new BooleanPresenter();
		}
	}
}