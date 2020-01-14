using System.Windows;

namespace Blastic.Wpf.DynamicControls.Presenters
{
	public class PasswordPresenter : Presenter
	{
		static PasswordPresenter()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(PasswordPresenter), new FrameworkPropertyMetadata(typeof(PasswordPresenter)));
		}
	}
}