using Avalonia;

namespace Blastic.Avalonia.Sample
{
	public class Program
	{
		public static void Main(string[] args)
		{
			BuildAvaloniaApp()
				.StartWithClassicDesktopLifetime(args);
		}
		
		public static AppBuilder BuildAvaloniaApp()
		{
			return AppBuilder.Configure<App>()
				.UsePlatformDetect()
				.LogToTrace();
		}
	}
}
