using SkiaSharp;

namespace Blastic.Skia.Input;

public record struct PanEventArgs(SKPoint Position, InputSource Source, double Force);
public record struct TapEventArgs(SKPoint Position, InputSource Source);

public record struct PointerMoveEventArgs(SKPoint Position);
public record struct PointerPressEventArgs(SKPoint Position, MouseButton Button);
public record struct PointerReleaseEventArgs(SKPoint Position, MouseButton Button);
