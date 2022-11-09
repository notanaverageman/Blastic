using SkiaSharp.Views.Maui.Handlers;

namespace Blastic.Skia.Maui.TouchCanvas;

public partial class TouchCanvasViewHandler : SKCanvasViewHandler
{
	public static readonly PropertyMapper<TouchCanvasView, TouchCanvasViewHandler> TouchCanvasViewMapper = new (SKCanvasViewMapper)
	{
		[nameof(TouchCanvasView.EnableTouch)] = MapEnableTouch,
	};

	public TouchCanvasViewHandler() : base(TouchCanvasViewMapper, null)
	{
	}

	public TouchCanvasViewHandler(PropertyMapper? mapper = null) : base(mapper ?? TouchCanvasViewMapper, null)
	{
	}
}