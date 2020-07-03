using Blastic.Wpf.Initialization;
using Blastic.Wpf.Material.UserInterface;
using Blastic.Wpf.UserInterface.TabbedMain;

namespace Blastic.Wpf.Material.Extensions
{
	public static class ApplicationExtensions
	{
		public static BlasticApplicationBuilder AddMaterialDesign(this BlasticApplicationBuilder builder)
		{
			return builder.AddTypeMapper<TabbedMainViewModel, MaterialTabbedMainView>();
		}
	}
}