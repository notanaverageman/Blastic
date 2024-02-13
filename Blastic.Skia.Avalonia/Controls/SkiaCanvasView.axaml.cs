using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Skia;
using Avalonia.Threading;
using Blastic.Platform;
using Blastic.Skia.Input;
using SkiaSharp;
using MouseButton = Avalonia.Input.MouseButton;

namespace Blastic.Skia.Avalonia.Controls;

public partial class SkiaCanvasView : UserControl
{
	public static readonly StyledProperty<bool> EnableScrollingProperty =
		AvaloniaProperty.Register<Border, bool>(nameof(EnableScrolling), defaultValue: true);

	public static readonly DirectProperty<SkiaCanvasView, SkiaCanvas?> SkiaCanvasProperty =
		AvaloniaProperty.RegisterDirect<SkiaCanvasView, SkiaCanvas?>(
			nameof(SkiaCanvas),
			x => x.SkiaCanvas,
			(x, y) => x.SkiaCanvas = y);

	public static readonly DirectProperty<SkiaCanvasView, float> MinimumZoomScaleProperty =
		AvaloniaProperty.RegisterDirect<SkiaCanvasView, float>(
			nameof(MinimumZoomScale),
			x => x.MinimumZoomScale,
			(x, y) => x.MinimumZoomScale = y);

	public static readonly DirectProperty<SkiaCanvasView, float> MaximumZoomScaleProperty =
		AvaloniaProperty.RegisterDirect<SkiaCanvasView, float>(
			nameof(MaximumZoomScale),
			x => x.MaximumZoomScale,
			(x, y) => x.MaximumZoomScale = y);

	public static readonly DirectProperty<SkiaCanvasView, float> RequestedZoomScaleProperty =
		AvaloniaProperty.RegisterDirect<SkiaCanvasView, float>(
			nameof(RequestedZoomScale),
			x => x.RequestedZoomScale,
			(x, y) => x.RequestedZoomScale = y);

	static SkiaCanvasView()
	{
		SkiaCanvasProperty.Changed.Subscribe(x =>
		{
			SkiaCanvasView skiaCanvasView = (SkiaCanvasView)x.Sender;
			skiaCanvasView.OnSkiaCanvasChanged(x.OldValue.Value, x.NewValue.Value);
		});
	}

	private SkiaCanvas? _skiaCanvas;
	private IDisposable? _canvasSubscriptions;
	private bool _isUserScrolling;
	private bool _isUserZooming;

	private float _minimumZoomScale = 1;
	private float _maximumZoomScale = 1;
	private float _requestedZoomScale = 1;

	public bool EnableScrolling
	{
		get => GetValue(EnableScrollingProperty);
		set => SetValue(EnableScrollingProperty, value);
	}

	public SkiaCanvas? SkiaCanvas
	{
		get => _skiaCanvas;
		set => SetAndRaise(SkiaCanvasProperty, ref _skiaCanvas, value);
	}

	public float MinimumZoomScale
	{
		get => _minimumZoomScale;
		set => SetAndRaise(MinimumZoomScaleProperty, ref _minimumZoomScale, value);
	}

	public float MaximumZoomScale
	{
		get => _maximumZoomScale;
		set => SetAndRaise(MaximumZoomScaleProperty, ref _maximumZoomScale, value);
	}

	public float RequestedZoomScale
	{
		get => _requestedZoomScale;
		set => SetAndRaise(RequestedZoomScaleProperty, ref _requestedZoomScale, value);
	}

	public SkiaCanvasView()
	{
		InitializeComponent();

		Canvas.Draw += OnDraw;
		Canvas.PointerWheelChanged += OnWheelChanged;

		ScrollViewer.ScrollChanged += OnScrollChanged;
	}

	private void OnDraw(object? sender, SKCanvasEventArgs e)
	{
		SkiaCanvas?.Draw(e.Canvas);
	}

	private void OnWheelChanged(object? sender, PointerWheelEventArgs e)
	{
		if (!e.KeyModifiers.HasFlag(KeyModifiers.Control))
		{
			return;
		}

		SkiaCanvas? canvas = SkiaCanvas;

		if (canvas == null)
		{
			return;
		}

		if (e.Delta.Y == 0)
		{
			return;
		}

		e.Handled = true;

		_isUserZooming = true;

		float scale = e.Delta.Y < 0
			? 0.9f
			: 1.1f;

		Point scrollViewerPosition = e.GetPosition(sender as Control);
		SKPoint contentPosition = canvas.GetContentPosition(scrollViewerPosition.ToSKPoint());

		canvas.Scale.Value *= scale;

		SKPoint newScrollViewerPosition = canvas.GetCanvasPosition(contentPosition);

		float scrollX = (float)(newScrollViewerPosition.X - scrollViewerPosition.X);
		float scrollY = (float)(newScrollViewerPosition.Y - scrollViewerPosition.Y);
		
		canvas.ScrollX.Value += scrollX;
		canvas.ScrollY.Value += scrollY;

		canvas.ScrollX.Value = Math.Min(canvas.MaxScrollX.Value, Math.Max(canvas.ScrollX.Value, 0));
		canvas.ScrollY.Value = Math.Min(canvas.MaxScrollY.Value, Math.Max(canvas.ScrollY.Value, 0));

		canvas.Redraw();

		_isUserZooming = false;
	}

	private void OnScrollChanged(object? sender, ScrollChangedEventArgs e)
	{
		if (_isUserZooming)
		{
			return;
		}

		if (e.OffsetDelta.Length == 0)
		{
			return;
		}

		SkiaCanvas? canvas = SkiaCanvas;

		if (canvas == null)
		{
			return;
		}

		_isUserScrolling = true;

		canvas.ScrollX.Value = (float)ScrollViewer.Offset.X;
		canvas.ScrollY.Value = (float)ScrollViewer.Offset.Y;

		canvas.Redraw();

		_isUserScrolling = false;
	}

	private void SkiaCanvasOnRedrawRequested(object? sender, EventArgs eventArgs)
	{
		Dispatcher.UIThread.InvokeAsync(() =>
		{
			Canvas.InvalidateVisual();
		});
	}

	private void ScroolTo(float scrollX, float scrollY)
	{
		if (_isUserScrolling)
		{
			return;
		}
		
		ScrollViewer.Offset = new Vector(scrollX, scrollY);
	}

	private void OnPointerMoved(object? sender, PointerEventArgs args)
	{
		Point position = args.GetPosition(Canvas);
		PointerMoveEventArgs eventArgs = new(position.ToSKPoint());

		SkiaCanvas?.InputEvents.FirePointerMove(eventArgs);
	}

	private void OnPointerPressed(object? sender, PointerPressedEventArgs args)
	{
		Point position = args.GetPosition(Canvas);
		Input.MouseButton mouseButton = GetMouseButton(args.GetCurrentPoint(null));

		PointerPressEventArgs eventArgs = new(position.ToSKPoint(), mouseButton);
		SkiaCanvas?.InputEvents.FirePointerPress(eventArgs);
	}

	private void OnPointerReleased(object? sender, PointerReleasedEventArgs args)
	{
		Point position = args.GetPosition(Canvas);
		Input.MouseButton mouseButton = GetMouseButton(args.GetCurrentPoint(null));

		PointerReleaseEventArgs eventArgs = new(position.ToSKPoint(), mouseButton);
		SkiaCanvas?.InputEvents.FirePointerRelease(eventArgs);
	}

	private void OnSkiaCanvasChanged(SkiaCanvas? oldValue, SkiaCanvas? newValue)
	{
		_canvasSubscriptions?.Dispose();
		_canvasSubscriptions = null;

		if (oldValue != null)
		{
			oldValue.RedrawRequested -= SkiaCanvasOnRedrawRequested;
		}

		if (newValue == null)
		{
			return;
		}
		
		newValue.RedrawRequested += SkiaCanvasOnRedrawRequested;

		IDisposable sizeSubscription = newValue.CanvasContentSize
			.Where(x => !x.IsEmpty)
			.ObserveOnUI()
			.Subscribe(x =>
			{
				ScrollViewerSizer.Width = x.Width;
				ScrollViewerSizer.Height = x.Height;
			});

		IDisposable scrollSubscription = newValue.ScrollX
			.CombineLatest(newValue.ScrollY)
			.ObserveOnUI()
			.Subscribe(x => ScroolTo(x.First, x.Second));

		IDisposable inputEnabledSubscription = newValue.InputEvents.IsEnabled
			.Subscribe(isEnabled =>
			{
				if (isEnabled)
				{
					Canvas.PointerMoved += OnPointerMoved;
					Canvas.PointerPressed += OnPointerPressed;
					Canvas.PointerReleased += OnPointerReleased;
				}
				else
				{
					Canvas.PointerMoved -= OnPointerMoved;
					Canvas.PointerPressed -= OnPointerPressed;
					Canvas.PointerReleased -= OnPointerReleased;
				}
			});

		_canvasSubscriptions = new CompositeDisposable(
			sizeSubscription,
			scrollSubscription,
			inputEnabledSubscription);
	}

	private Input.MouseButton GetMouseButton(PointerPoint point)
	{
		return point.Properties.PointerUpdateKind.GetMouseButton() switch
		{
			MouseButton.Left => Input.MouseButton.Left,
			MouseButton.Right => Input.MouseButton.Right,
			MouseButton.Middle => Input.MouseButton.Middle,
			_ => Input.MouseButton.Left
		};
	}
}