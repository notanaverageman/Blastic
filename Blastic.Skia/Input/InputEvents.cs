using System.Reactive.Linq;
using Blastic.Reactive;
using SkiaSharp;

namespace Blastic.Skia.Input;

public class InputEvents
{
	private readonly SkiaCanvas _canvas;
	
	public IReactiveProperty<bool> IsEnabled { get; }

	public event EventHandler<PanEventArgs>? Panned;
	public event EventHandler<TapEventArgs>? Tapped;
	public event EventHandler<PointerMoveEventArgs>? PointerMoved;
	public event EventHandler<PointerPressEventArgs>? PointerPressed;
	public event EventHandler<PointerReleaseEventArgs>? PointerReleased;

	public IObservable<PanEventArgs> Pan { get; }
	public IObservable<TapEventArgs> Tap { get; }
	public IObservable<PointerMoveEventArgs> PointerMove { get; }
	public IObservable<PointerPressEventArgs> PointerPress { get; }
	public IObservable<PointerReleaseEventArgs> PointerRelease { get; }

	public InputEvents(SkiaCanvas canvas)
	{
		_canvas = canvas;

		IsEnabled = new ReactiveProperty<bool>(false);

		Pan = Observable
			.FromEventPattern<PanEventArgs>(
				x => Panned += x,
				x => Panned -= x)
			.Select(x => x.EventArgs);

		Tap = Observable
			.FromEventPattern<TapEventArgs>(
				x => Tapped += x,
				x => Tapped -= x)
			.Select(x => x.EventArgs);

		PointerMove = Observable
			.FromEventPattern<PointerMoveEventArgs>(
				x => PointerMoved += x,
				x => PointerMoved -= x)
			.Select(x => x.EventArgs);

		PointerPress = Observable
			.FromEventPattern<PointerPressEventArgs>(
				x => PointerPressed += x,
				x => PointerPressed -= x)
			.Select(x => x.EventArgs);

		PointerRelease = Observable
			.FromEventPattern<PointerReleaseEventArgs>(
				x => PointerReleased += x,
				x => PointerReleased -= x)
			.Select(x => x.EventArgs);
	}

	public void FirePan(PanEventArgs args)
	{
		SKPoint contentPosition = GetPosition(args.Position);
		args = args with { Position = contentPosition };

		Panned?.Invoke(this, args);
	}

	public void FireTap(TapEventArgs args)
	{
		SKPoint contentPosition = GetPosition(args.Position);
		args = args with { Position = contentPosition };

		Tapped?.Invoke(this, args);
	}

	public void FirePointerMove(PointerMoveEventArgs args)
	{
		SKPoint contentPosition = GetPosition(args.Position);
		args = args with { Position = contentPosition };

		PointerMoved?.Invoke(this, args);
	}

	public void FirePointerPress(PointerPressEventArgs args)
	{
		SKPoint contentPosition = GetPosition(args.Position);
		args = args with { Position = contentPosition };

		PointerPressed?.Invoke(this, args);
	}
	
	public void FirePointerRelease(PointerReleaseEventArgs args)
	{
		SKPoint contentPosition = GetPosition(args.Position);
		args = args with { Position = contentPosition };

		PointerReleased?.Invoke(this, args);
	}
	
	private SKPoint GetPosition(SKPoint position)
	{
		return _canvas.GetContentPosition(position);
	}
}