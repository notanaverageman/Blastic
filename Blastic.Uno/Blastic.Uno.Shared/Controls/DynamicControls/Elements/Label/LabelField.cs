using Blastic.Reactive;
using Blastic.Uno.Shared.Controls.DynamicControls.Elements.Label;

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