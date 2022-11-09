using Blastic.Skia.Maui.Scrolling;
using Blastic.Skia.Maui.TouchCanvas;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace Blastic.Skia.Maui.Initialization;

public static class MauiAppBuilderExtensions
{
	public static MauiAppBuilder UseBlasticSkia(this MauiAppBuilder builder)
	{
		builder.UseSkiaSharp();

		return builder.ConfigureMauiHandlers(handlers =>
		{
			handlers.AddHandler(typeof(FixedContentScrollView), typeof(FixedContentScrollViewHandler));
			handlers.AddHandler(typeof(TouchCanvasView), typeof(TouchCanvasViewHandler));
		});
	}
}