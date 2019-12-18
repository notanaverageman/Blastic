using System.Windows;
using System.Windows.Controls;
using Blastic.Reactive;
using MaterialDesignThemes.Wpf;

namespace Blastic.Controls.DynamicControls.Elements
{
	public class Presenter : Control
	{
		public IReactiveProperty Property { get; set; }

		public IReactiveProperty<string> Help { get; set; }
		public IReactiveProperty<string> Label { get; set; }
		public IReactiveProperty<PackIconKind?> IconKind { get; set; }

		public IReactiveProperty<bool> IsEnabledReactive { get; set; }

		public GridLength ColumnWidth { get; set; }
		public Thickness IconMargin { get; set; }
		public double IconSize { get; set; }
	}
}