using System;
using System.Threading.Tasks;
using System.Windows;

namespace Blastic.Services.Windowing
{
	public interface IWindowManager
	{
		Task ShowWindow(object rootModel, Action<Window> configure = null);
	}
}