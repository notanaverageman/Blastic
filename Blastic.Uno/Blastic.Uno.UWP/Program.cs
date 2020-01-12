using Windows.ApplicationModel.Resources;
using Autofac;
using Blastic.Common;
using Blastic.Initialization;
using Blastic.Initialization.Extensions;
using Blastic.UserInterface.TabbedMain;

namespace Blastic.Uno
{
	public class Program
	{
		public static void Main()
		{
			ProductInformation productInformation = new ProductInformation();

			productInformation.ProgramName.Value = "Blastic Sample Application";
			productInformation.Version.Value = typeof(Program).Assembly.GetName().Version;

			new BlasticApplication()
				.Configure(x => x.RegisterInstance(productInformation))
				.RegisterViewAssembly<Program>()
				.RegisterSettingsAssembly<Program>()
				.RegisterInitializationStepsAssembly<Program>()
				.AddLocalizationSource(ResourceLoader.GetForViewIndependentUse())
				.RegisterMainTabs<Program>()
				// .AddLogsWindow()
				// .AddSettingsWindow()
				.Run<TabbedMainViewModel>();
		}
	}
}