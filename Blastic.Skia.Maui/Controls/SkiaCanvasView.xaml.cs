using System.Reactive.Disposables;
using System.Reactive.Linq;
using Bindables.Maui;
using Blastic.Platform;
using Blastic.Skia.Input;
using Blastic.Skia.Maui.Scrolling;
using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace Blastic.Skia.Maui.Controls;

public partial class SkiaCanvasView : ContentView
{
	private const bool EnableScrollingDefaultValue = true;
	private const float MinimumZoomScaleDefaultValue = 1;
	private const float MaximumZoomScaleDefaultValue = 1;

	[BindableProperty(typeof(SkiaCanvas), OnPropertyChanged = nameof(SkiaCanvasChanged))]
	public static readonly BindableProperty SkiaCanvasProperty;

	[BindableProperty(typeof(bool), DefaultValueField = nameof(EnableScrollingDefaultValue))]
	public static readonly BindableProperty EnableScrollingProperty;

	[BindableProperty(typeof(float), DefaultValueField = nameof(MinimumZoomScaleDefaultValue))]
	public static readonly BindableProperty MinimumZoomScaleProperty;

	[BindableProperty(typeof(float), DefaultValueField = nameof(MaximumZoomScaleDefaultValue))]
	public static readonly BindableProperty MaximumZoomScaleProperty;
	
	private IDisposable? _canvasSubscriptions;
	private bool _isUserScrolling;
	private bool _isUserZooming;

	public FixedContentScrollView ScrollView => ScrollViewXaml;

	public SkiaCanvasView()
	{
		InitializeComponent();

		Loaded += (_, _) =>
		{
			DeviceDisplay.MainDisplayInfoChanged += OnDisplayInfoChanged;

			double density = DeviceDisplay.MainDisplayInfo.Density;
			SkiaCanvas?.SetScreenDensity((float)density);
		};

		Unloaded += (_, _) =>
		{
			DeviceDisplay.MainDisplayInfoChanged -= OnDisplayInfoChanged;
		};
	}

	private void OnDisplayInfoChanged(object? sender, DisplayInfoChangedEventArgs args)
	{
		SkiaCanvas?.SetScreenDensity((float)args.DisplayInfo.Density);
	}

	public void Redraw()
	{
		PlatformSpecifics.Current.OnUIThread(() =>
		{
			Canvas.InvalidateSurface();
		});
	}

	private void OnSizeChanged(object? sender, EventArgs e)
	{
		Canvas.WidthRequest = SizeProvider.Width;
		Canvas.HeightRequest = SizeProvider.Height;
	}

	private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
	{
		SKCanvas canvas = e.Surface.Canvas;
		SkiaCanvas?.Draw(canvas);
	}
	
	private void OnViewportChanged(object sender, ViewportChangedEventArgs args)
	{
		SkiaCanvas? canvas = SkiaCanvas;

		if (canvas == null)
		{
			return;
		}

		_isUserZooming = true;
		_isUserScrolling = true;

		canvas.ScrollX.Value = (float)args.ScrollX;
		canvas.ScrollY.Value = (float)args.ScrollY;
		canvas.Scale.Value = (float)args.ZoomScale;

		canvas.Redraw();

		_isUserScrolling = false;
		_isUserZooming = false;
	}

	private void OnTap(object? sender, TapEventArgs args)
	{
		SkiaCanvas?.InputEvents.FireTap(args);
	}

	private void OnPan(object? sender, PanEventArgs args)
	{
		SkiaCanvas?.InputEvents.FirePan(args);
	}

	private void OnPointerMoved(object? sender, PointerMoveEventArgs args)
	{
		SkiaCanvas?.InputEvents.FirePointerMove(args);
	}

	private void OnPointerPressed(object? sender, PointerPressEventArgs args)
	{
		SkiaCanvas?.InputEvents.FirePointerPress(args);
	}

	private void OnPointerReleased(object? sender, PointerReleaseEventArgs args)
	{
		SkiaCanvas?.InputEvents.FirePointerRelease(args);
	}

	private void ScroolTo(float scrollX, float scrollY)
	{
		if (_isUserScrolling)
		{
			return;
		}

		ScrollView.ScrollToAsync(scrollX, scrollY, false);
	}

	private void ZoomTo(float scale)
	{
		if (_isUserZooming)
		{
			return;
		}

		ScrollView.RequestedZoomScale = scale;
	}

	private void SkiaCanvasOnRedrawRequested(object? sender, EventArgs args)
	{
		Redraw();
	}

	private static void SkiaCanvasChanged(BindableObject bindable, object oldValue, object newValue)
	{
		SkiaCanvasView skiaCanvasView = (SkiaCanvasView)bindable;

		skiaCanvasView._canvasSubscriptions?.Dispose();
		skiaCanvasView._canvasSubscriptions = null;

		if (oldValue is SkiaCanvas oldSkiaCanvas)
		{
			oldSkiaCanvas.RedrawRequested -= skiaCanvasView.SkiaCanvasOnRedrawRequested;
		}

		if (newValue is SkiaCanvas newSkiaCanvas)
		{
			newSkiaCanvas.RedrawRequested += skiaCanvasView.SkiaCanvasOnRedrawRequested;

			IDisposable sizeSubscription = newSkiaCanvas.CanvasContentSize
				.Where(x => !x.IsEmpty)
				.Subscribe(x =>
				{
					skiaCanvasView.ScrollView.ContentWidth = x.Width;
					skiaCanvasView.ScrollView.ContentHeight = x.Height;
				});

			IDisposable scrollSubscription = newSkiaCanvas.ScrollX
				.CombineLatest(newSkiaCanvas.ScrollY)
				.Subscribe(x => skiaCanvasView.ScroolTo(x.First, x.Second));

			IDisposable scaleSubscription = newSkiaCanvas.Scale
				.Subscribe(x => skiaCanvasView.ZoomTo(x));

			IDisposable inputEnabledSubscription = newSkiaCanvas.InputEvents.IsEnabled
				.Subscribe(isEnabled =>
				{
					if (isEnabled)
					{
						skiaCanvasView.Canvas.EnableTouch = true;

						skiaCanvasView.Canvas.Tapped += skiaCanvasView.OnTap;
						skiaCanvasView.Canvas.Panned += skiaCanvasView.OnPan;
						skiaCanvasView.Canvas.PointerMoved += skiaCanvasView.OnPointerMoved;
						skiaCanvasView.Canvas.PointerPressed += skiaCanvasView.OnPointerPressed;
						skiaCanvasView.Canvas.PointerReleased += skiaCanvasView.OnPointerReleased;
					}
					else
					{
						skiaCanvasView.Canvas.EnableTouch = false;

						skiaCanvasView.Canvas.Tapped -= skiaCanvasView.OnTap;
						skiaCanvasView.Canvas.Panned -= skiaCanvasView.OnPan;
						skiaCanvasView.Canvas.PointerMoved -= skiaCanvasView.OnPointerMoved;
						skiaCanvasView.Canvas.PointerPressed -= skiaCanvasView.OnPointerPressed;
						skiaCanvasView.Canvas.PointerReleased -= skiaCanvasView.OnPointerReleased;
					}
				});

			skiaCanvasView._canvasSubscriptions = new CompositeDisposable(
				sizeSubscription,
				scrollSubscription,
				scaleSubscription,
				inputEnabledSubscription);
		}
	}
}