using System.Threading.Tasks;

namespace Blastic.Wpf.Services.Windowing
{
	public interface IWindowManager
	{
		Task ShowWindow(object rootModel);
	}
}