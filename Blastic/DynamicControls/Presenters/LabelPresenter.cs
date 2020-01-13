using System.Windows;

namespace Blastic.DynamicControls.Presenters
{
	public class LabelPresenter : Presenter
	{
		static LabelPresenter()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(LabelPresenter), new FrameworkPropertyMetadata(typeof(LabelPresenter)));
		}
	}
}