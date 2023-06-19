using Blastic.Maui.Initialization;
using Blastic.Maui.Sample.Data;
using Blastic.Skia;
using Blastic.ViewManagement.TypeMappers;
using Depso;

namespace Blastic.Maui.Sample;

[ServiceProvider]
public partial class ServiceProvider
{
	private void RegisterServices()
	{
		ImportModule<BlasticServices>();

		AddSingleton<App>();

		AddSingleton<MainViewModel>();
		AddSingleton<MainView>();
		AddSingleton<ITypeMapper, DirectTypeMapper<MainViewModel, MainView>>();

		AddSingleton<Board>();
		AddSingleton<SkiaCanvas>();
		AddSingleton<Game>();
		AddSingleton<GameDatabase>();
		AddSingleton<GameActionManager>();
		AddSingleton(_ => new GameDatabaseOptions
		{
			ConnectionStringBuilder =
			{
				DataSource = Path.Combine(FileSystem.AppDataDirectory, "games.db")
			}
		});
	}
}