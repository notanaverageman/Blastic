using System.Threading.Tasks;
using Blastic.Wpf.Sample.Properties;
using Blastic.Wpf.Initialization.Extensions;
using Blastic.Wpf.Sample.UserInterface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Blastic.Wpf.Sample
{
	public class Program
	{
		public static async Task Main()
		{
			IHost host = new HostBuilder()
				.ConfigureAppConfiguration(x => x.AddJsonFile("AppSettings.json"))
				.ConfigureBlasticApplication(
					x => x
						.UseApplication<App>()
						.UseMainViewModel<MainViewModel>()
						.AddLocalizationSource(Resources.ResourceManager))
				.Build();

			await host.RunAsync();
		}
	}
}