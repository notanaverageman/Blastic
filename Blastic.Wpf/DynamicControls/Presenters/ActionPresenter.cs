using System.Windows;
using System.Windows.Input;

namespace Blastic.Wpf.DynamicControls.Presenters
{
	public class ActionPresenter : Presenter
	{
		static ActionPresenter()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ActionPresenter), new FrameworkPropertyMetadata(typeof(ActionPresenter)));
		}

		public ICommand? Command { get; set; }
	}
}