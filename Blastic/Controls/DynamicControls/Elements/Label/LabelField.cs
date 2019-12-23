using Blastic.Reactive;

namespace Blastic.Controls.DynamicControls.Elements.Label
{
	public class LabelField : Field
	{
		public LabelField(IReactiveProperty property) : base(property)
		{
		}

		protected override Presenter CreatePresenter()
		{
			return new LabelPresenter();
		}
	}
}