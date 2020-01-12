using Windows.UI.Xaml;
using Blastic.Reactive;
using Blastic.Uno.Shared.Controls.DynamicControls.Elements.Selection;

namespace Blastic.Controls.DynamicControls.Elements.Selection
{
	public class SelectionField<T> : Field
	{
		public ReactiveCollection<T> Values { get; }

		public SelectionField(IReactiveProperty<T> property, ReactiveCollection<T> values) : base(property)
		{
			Values = values;
			IconMargin = new Thickness(0, 16, 8, 0);
		}

		protected override Presenter CreatePresenter()
		{
			return new SelectionPresenter
			{
				Values = Values
			};
		}
	}
}