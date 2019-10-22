using System.Windows;

namespace Blastic.Controls.DynamicControls.Elements.Boolean
{
	public class BooleanPresenter : Presenter
	{
		static BooleanPresenter()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BooleanPresenter), new FrameworkPropertyMetadata(typeof(BooleanPresenter)));
		}
	}
}