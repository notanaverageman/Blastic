using System.Threading.Tasks;

namespace Blastic.Services.Windowing;

public interface IWindowService
{
	Task ShowWindow(object viewModel);
}