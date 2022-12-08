using System.Windows.Controls;
using Blastic.DynamicControls;
using Blastic.DynamicControls.Properties;
using Blastic.Reactive;

namespace Blastic.Wpf.DynamicControls.Presenters
{
	public class Presenter : Control, IPresenter
	{
		public IReadOnlyReactiveProperty? Property { get; set; }

		public IReadOnlyReactiveProperty<string?>? Help { get; set; }
		public IReadOnlyReactiveProperty<string?>? Label { get; set; }
		public IReadOnlyReactiveProperty? Icon { get; set; }

		public IReadOnlyReactiveProperty<bool>? IsEnabledReactive { get; set; }

		public GridLength ColumnWidth { get; set; }
		public Thickness IconMargin { get; set; }
		public double IconSize { get; set; }
	}
}