using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Blastic.Reactive;

namespace Blastic.Controls.DynamicControls.Elements
{
	public class Presenter : ContentControl
	{
		public IReactiveProperty Property { get; set; }

		public IReactiveProperty<string> Help { get; set; }
		public IReactiveProperty<string> Label { get; set; }
		public IReactiveProperty<Symbol?> Icon { get; set; }

		public IReactiveProperty<bool> IsEnabledReactive { get; set; }

		public GridLength ColumnWidth { get; set; }
		public Thickness IconMargin { get; set; }
		public double IconSize { get; set; }

		public Presenter()
		{
			HorizontalContentAlignment = HorizontalAlignment.Stretch;
		}
	}
}