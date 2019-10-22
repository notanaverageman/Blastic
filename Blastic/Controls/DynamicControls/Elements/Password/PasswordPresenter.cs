using System.Windows;

namespace Blastic.Controls.DynamicControls.Elements.Password
{
	public class PasswordPresenter : Presenter
	{
		static PasswordPresenter()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(PasswordPresenter), new FrameworkPropertyMetadata(typeof(PasswordPresenter)));
		}
	}
}