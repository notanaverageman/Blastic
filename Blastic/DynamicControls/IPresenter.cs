using Blastic.DynamicControls.Properties;
using Blastic.Reactive;

namespace Blastic.DynamicControls
{
	public interface IPresenter
	{
		IReadOnlyReactiveProperty? Property { get; set; }

		IReadOnlyReactiveProperty<string?>? Help { get; set; }
		IReadOnlyReactiveProperty<string?>? Label { get; set; }
		IReadOnlyReactiveProperty? Icon { get; set; }

		IReadOnlyReactiveProperty<bool>? IsEnabledReactive { get; set; }

		GridLength ColumnWidth { get; set; }
		Thickness IconMargin { get; set; }
		double IconSize { get; set; }

		double MinWidth { get; set; }
		double MinHeight { get; set; }
	}
}