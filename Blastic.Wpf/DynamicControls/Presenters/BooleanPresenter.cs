using System.Windows;

namespace Blastic.DynamicControls.Presenters
{
	public class BooleanPresenter : Presenter
	{
		static BooleanPresenter()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BooleanPresenter), new FrameworkPropertyMetadata(typeof(BooleanPresenter)));
		}
	}
}