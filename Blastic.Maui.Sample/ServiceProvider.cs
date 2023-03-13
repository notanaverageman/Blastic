using Jab;
using Blastic.Maui.Initialization;
using Blastic.Maui.Sample.Data;
using Blastic.Skia;
using Blastic.ViewManagement.TypeMappers;

namespace Blastic.Maui.Sample;

[Import<IBlasticServices>]

[Singleton<App>]

[Singleton<MainViewModel>]
[Singleton<MainView>]
[Singleton<ITypeMapper>(Factory = nameof(MainView))]

[Singleton<Board>]
[Singleton<SkiaCanvas>]
[Singleton<Game>]
[Singleton<GameDatabase>]
[Singleton<GameDatabaseOptions>(Factory = nameof(GameDatabaseOptions))]
[Singleton<GameActionManager>]

[ServiceProvider]
public partial class ServiceProvider
{
	private IServiceScopeFactory CreateScopeFactory() => this;

	private	static ITypeMapper MainView() => new DirectTypeMapper<MainViewModel, MainView>();

	private static GameDatabaseOptions GameDatabaseOptions() => new()
	{
		ConnectionStringBuilder =
		{
			DataSource = Path.Combine(FileSystem.AppDataDirectory, "games.db")
		}
	};
}