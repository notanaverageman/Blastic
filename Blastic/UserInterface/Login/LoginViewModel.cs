using Blastic.Execution;
using Blastic.LifetimeManagement;

namespace Blastic.UserInterface.Login
{
	public class LoginViewModel : Screen
	{
		public LoginViewModel(ExecutionContextFactory executionContextFactory) : base(executionContextFactory)
		{
		}
	}
}