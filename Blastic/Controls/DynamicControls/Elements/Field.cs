using Reactive.Bindings;

namespace Blastic.Controls.DynamicControls.Elements
{
	public interface IField : IElement
	{
		IReactiveProperty Property { get; }
	}

	public abstract class Field : Element, IField
	{
		public IReactiveProperty Property { get; }

		public Field(IReactiveProperty property)
		{
			Property = property;
		}

		public override Presenter ToPresenter()
		{
			Presenter presenter = base.ToPresenter();
			presenter.Property = Property;

			return presenter;
		}
	}
}