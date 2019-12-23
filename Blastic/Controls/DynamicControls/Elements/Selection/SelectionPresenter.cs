using System.Collections;
using System.Windows;

namespace Blastic.Controls.DynamicControls.Elements.Selection
{
	public class SelectionPresenter : Presenter
	{
		static SelectionPresenter()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(SelectionPresenter), new FrameworkPropertyMetadata(typeof(SelectionPresenter)));
		}

		public IEnumerable Values { get; set; }
	}
}