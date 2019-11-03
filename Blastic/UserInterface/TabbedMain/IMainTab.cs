using Blastic.Common;
using Blastic.LifetimeManagement;

namespace Blastic.UserInterface.TabbedMain
{
	public interface IMainTab : IHasLifetime
	{
		Order Order { get; }
		bool IsFixed { get; }
	}
}