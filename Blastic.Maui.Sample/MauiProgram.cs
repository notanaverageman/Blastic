using Blastic.Maui.Initialization;
using Blastic.Skia.Maui.Initialization;

namespace Blastic.Maui.Sample;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		ServiceProvider serviceProvider = new();

		MauiAppBuilder builder = MauiApp.CreateBuilder();

		builder
			.UseBlastic<App, MainViewModel>(serviceProvider)
			.UseBlasticSkia();

		return builder.Build();
	}
}