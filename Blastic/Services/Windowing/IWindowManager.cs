using System.Threading.Tasks;

namespace Blastic.Services.Windowing
{
	public interface IWindowManager
	{
		Task ShowWindow(object rootModel);
	}
}