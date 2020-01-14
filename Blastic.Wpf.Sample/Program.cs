using System;
using Blastic.Data;
using Blastic.Initialization;
using Blastic.Wpf.Sample.Properties;
using Blastic.Wpf.Data;
using Blastic.Wpf.Initialization;
using Blastic.Wpf.Initialization.Extensions;
using Blastic.Wpf.Sample.UserInterface;
using Blastic.Wpf.UserInterface.TabbedMain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Blastic.Wpf.Sample
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
				.Configure(x => x.AddSingleton(productInformation))
				.Configure(x => x.AddJsonFile("AppSettings.json"))
				.RegisterViewAssembly<Program>()
				.AddSetting<TestSettingsViewModel>()
				.AddMainTab<HomeViewModel>()
				.AddMainTab<MainTabViewModel>()
				.AddLogsWindow()
				.AddSettingsWindow()
				.AddSettingsService()
				.AddLocalizationSource(Resources.ResourceManager)
				.AddProgramDatabase(DatabaseProvider.SQLite, "Data Source=Settings.sqlite;")
				.Run<App, TabbedMainViewModel>();
		}
	}
}