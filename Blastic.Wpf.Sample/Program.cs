using System.Threading.Tasks;
using Blastic.Data;
using Blastic.Wpf.Initialization;
using Blastic.Wpf.Sample.Properties;
using Blastic.Wpf.Initialization.Extensions;
using Blastic.Wpf.Material.Extensions;
using Blastic.Wpf.Sample.UserInterface;
using Blastic.Wpf.Services.Settings;
using Blastic.Wpf.UserInterface.TabbedMain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Blastic.Wpf.Sample
{
	public class Program
	{
		public static async Task Main()
		{
			ProductInformation productInformation = new ProductInformation();

			productInformation.ProgramName.Value = "Blastic Sample Application";
			productInformation.Version.Value = typeof(Program).Assembly.GetName().Version;

			IHost host = new HostBuilder()
				.ConfigureServices(x =>
				{
					x.AddSingleton(productInformation);
				})
				.ConfigureAppConfiguration(x => x.AddJsonFile("AppSettings.json"))
				.ConfigureBlasticApplication(
					x => x
						.UseApplication<App>()
						.UseMainViewModel<TabbedMainViewModel>()
						.AddMaterialDesign()
						.AddSetting<TestSettingsViewModel>()
						.AddMainTab<HomeViewModel>()
						.AddMainTab<MainTabViewModel>()
						.AddLogsWindow()
						.AddSettingsService<DatabaseSettingsStorage>()
						.AddProgramDatabase(DatabaseProvider.SQLite, "Data Source=Settings.sqlite;")
						.AddLocalizationSource(Resources.ResourceManager))
				.Build();

			await host.RunAsync();
		}
	}
}