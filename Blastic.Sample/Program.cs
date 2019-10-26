using System;
using Blastic.Data;
using Blastic.Initialization;
using Blastic.Initialization.Extensions;
using Blastic.Sample.UserInterface;
using Blastic.UserInterface.TabbedMain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Blastic.Sample
{
	public class Program
	{
		[STAThread]
		public static void Main()
		{
			new BlasticApplication()
				.Configure(x => x.AddSingleton(y =>
				{
					ProductInformation productInformation = new ProductInformation();

					productInformation.ProgramName.Value = "Blastic Sample Application";
					productInformation.Version.Value = typeof(Program).Assembly.GetName().Version;

					return productInformation;
				}))
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