using System.Collections;

namespace Blastic.Uno.Shared.Controls.DynamicControls.Elements.Selection
{
	public sealed partial class SelectionPresenter
	{
		public IEnumerable Values { get; set; }

		public SelectionPresenter()
		{
			InitializeComponent();
		}
	}
}