using Blastic.Wpf.Initialization;
using Blastic.Wpf.Material.UserInterface;
using Blastic.Wpf.UserInterface.TabbedMain;

namespace Blastic.Wpf.Material.Extensions
{
	public static class ApplicationExtensions
	{
		public static BlasticApplication AddMaterialDesign(this BlasticApplication application)
		{
			return application.Configure(x => x.WithTypeMapper<TabbedMainViewModel, MaterialTabbedMainView>());
		}
	}
}