using System;
using Autofac;
using Blastic.Data;
using Blastic.Initialization;
using Blastic.Initialization.Extensions;
using Blastic.UserInterface.TabbedMain;
using Microsoft.Extensions.Configuration;

namespace Blastic.Sample
{
	public class Program
	{
		[STAThread]
		public static void Main()
		{
			ProductInformation productInformation = new ProductInformation();

			productInformation.ProgramName.Value = "Blastic Sample Application";
			productInformation.Version.Value = typeof(Program).Assembly.GetName().Version;

			new BlasticApplication()
				.Configure(x => x.RegisterInstance(productInformation))
				.Configure(x => x.AddJsonFile("AppSettings.json"))
				.RegisterViewAssembly<Program>()
				.RegisterSettingsAssembly<Program>()
				.RegisterInitializationStepsAssembly<Program>()
				.RegisterMainTabs<Program>()
				.AddLogsWindow()
				.AddSettingsWindow()
				.AddSettingsService()
				.AddProgramDatabase(DatabaseProvider.SQLite, "Data Source=Settings.sqlite;")
				.Run<TabbedMainViewModel>();
		}
	}
}