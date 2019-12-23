using System.Windows;

namespace Blastic.Controls.DynamicControls.Elements.Label
{
	public class LabelPresenter : Presenter
	{
		static LabelPresenter()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(LabelPresenter), new FrameworkPropertyMetadata(typeof(LabelPresenter)));
		}
	}
}