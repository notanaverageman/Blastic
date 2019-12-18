using System.Windows;
using System.Windows.Input;

namespace Blastic.Controls.DynamicControls.Elements.Action
{
	public class ActionPresenter : Presenter
	{
		public ICommand Command { get; set; }

		static ActionPresenter()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ActionPresenter), new FrameworkPropertyMetadata(typeof(ActionPresenter)));
		}
	}
}