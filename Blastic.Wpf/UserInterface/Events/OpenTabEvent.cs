using Blastic.UserInterface.TabbedMain;

namespace Blastic.UserInterface.Events
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