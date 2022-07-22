using System.Threading.Tasks;
using Blastic.ViewManagement;

namespace Blastic.Maui.Services.Navigation;

public interface INavigationService
{
	Task GoBack(IViewAware parent);
	Task NavigateTo(IViewAware parent, object model);
}