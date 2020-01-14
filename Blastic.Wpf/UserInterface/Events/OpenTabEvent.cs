using Blastic.Wpf.UserInterface.TabbedMain;

namespace Blastic.Wpf.UserInterface.Events
{
	public class OpenTabEvent
	{
		public IMainTab ViewModel { get; }

		public OpenTabEvent(IMainTab viewModel)
		{
			ViewModel = viewModel;
		}
	}
}