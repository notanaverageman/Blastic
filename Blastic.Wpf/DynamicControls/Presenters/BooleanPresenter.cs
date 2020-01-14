using System.Windows;

namespace Blastic.Wpf.DynamicControls.Presenters
{
	public class BooleanPresenter : Presenter
	{
		static BooleanPresenter()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BooleanPresenter), new FrameworkPropertyMetadata(typeof(BooleanPresenter)));
		}
	}
}