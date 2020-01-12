using Autofac;
using Blastic.Initialization;
using Blastic.Initialization.Extensions;
using Blastic.UserInterface.TabbedMain;

namespace Blastic.Uno.Wasm
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
				.RegisterMainTabs<BlasticApplication>()
				// .AddLogsWindow()
				// .AddSettingsWindow()
				.Run<TabbedMainViewModel>();
		}
	}
}
