using Blastic.DynamicControls.Properties;
using Blastic.Reactive;

namespace Blastic.DynamicControls.Elements
{
	public class SelectionField<T> : Field
	{
		public ReactiveCollection<T> Values { get; }

		public SelectionField(IReactiveProperty<T> property, ReactiveCollection<T> values) : base(property)
		{
			Values = values;
			IconMargin = new Thickness(0, 16, 8, 0);
		}
	}
}