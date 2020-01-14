using System.Windows;
using System.Windows.Input;

namespace Blastic.DynamicControls.Presenters
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