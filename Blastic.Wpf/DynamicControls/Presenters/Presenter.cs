using System.Windows.Controls;
using Blastic.DynamicControls;
using Blastic.DynamicControls.Properties;
using Blastic.Reactive;

namespace Blastic.Wpf.DynamicControls.Presenters
{
	public class Presenter : Control, IPresenter
	{
		public IReactiveProperty Property { get; set; }

		public IReadOnlyReactiveProperty<string> Help { get; set; }
		public IReadOnlyReactiveProperty<string> Label { get; set; }
		public IReadOnlyReactiveProperty<object> Icon { get; set; }

		public IReactiveProperty<bool> IsEnabledReactive { get; set; }

		public GridLength ColumnWidth { get; set; }
		public Thickness IconMargin { get; set; }
		public double IconSize { get; set; }
	}
}