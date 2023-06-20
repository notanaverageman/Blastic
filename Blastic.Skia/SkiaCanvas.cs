using System.Numerics;
using System.Reactive.Linq;
using Blastic.Commanding;
using Blastic.Commanding.Concurrency;
using Blastic.Ordering;
using Blastic.Reactive;
using Blastic.Skia.Input;
using SkiaSharp;

namespace Blastic.Skia;

public class SkiaCanvas
{
	private readonly IReactiveProperty<SKSizeI> _canvasSize;
	private readonly IReadOnlyReactiveProperty<float> _canvasContentRatio;
	private readonly IReadOnlyReactiveProperty<SKPoint> _marginForCentering;

	private float _screenDensity;

	public IReadOnlyReactiveProperty<SKSize> ContentSizeScaled { get; }

	public event EventHandler? RedrawRequested;

	public CanvasFitMode FitMode { get; set; }

	public IReadOnlyReactiveProperty<SKSizeI> CanvasSize => _canvasSize;
	public IReadOnlyReactiveProperty<float> CanvasContentRatio => _canvasContentRatio;

	public IReadOnlyReactiveProperty<SKSize> CanvasContentSize { get; }

	public IReactiveProperty<SKSize> ContentSize { get; }
	public IReactiveProperty<float> GlobalContentScale { get; }

	public IReactiveProperty<float> Scale { get; }
	public IReactiveProperty<float> ScrollX { get; }
	public IReactiveProperty<float> ScrollY { get; }
	public IReadOnlyReactiveProperty<float> MaxScrollX { get; }
	public IReadOnlyReactiveProperty<float> MaxScrollY { get; }

	public IReactiveProperty<float> InsetRatio { get; }
	public IReadOnlyReactiveProperty<SKSize> InsetSize { get; }

	public Command<(SKCanvas Canvas, SKPaint? Paint)> DrawCommand { get; }

	public InputEvents InputEvents { get; }

	public SkiaCanvas()
	{
		_screenDensity = 1;
		_canvasSize = new ReactiveProperty<SKSizeI>(default);
		
		InputEvents = new InputEvents(this);

		FitMode = CanvasFitMode.ScaleToFitWidth;

		GlobalContentScale = new ReactiveProperty<float>(1);

		Scale = new ReactiveProperty<float>(1);
		ScrollX = new ReactiveProperty<float>(0);
		ScrollY = new ReactiveProperty<float>(0);

		ContentSize = new ReactiveProperty<SKSize>(default);
		InsetRatio = new ReactiveProperty<float>(0);

		ContentSizeScaled = ContentSize
			.CombineLatest(GlobalContentScale)
			.Select(x => new SKSize(x.First.Width * x.Second, x.First.Height * x.Second))
			.ToReadOnlyReactiveProperty(default);

		ResetTransformation();

		DrawCommand = new Command<(SKCanvas, SKPaint?)>()
			.WithReentrancy(new RunLatestCancelRunningReentrancyHandler())
			.WithSubscribe(BeforeDraw, Order.AbsoluteMinimum)
			.WithSubscribeFinally(AfterDraw, Order.AbsoluteMaximum);
		
		IObservable<(SKSize ContentSize, SKSizeI CanvasSize, float Scale, float InsetRatio)> sizesObservable = ContentSizeScaled
			.Where(x => !x.IsEmpty)
			.CombineLatest(_canvasSize.Where(x => !x.IsEmpty), Scale, InsetRatio);

		_canvasContentRatio = sizesObservable
			.Select(GetCanvasContentRatio)
			.ToReadOnlyReactiveProperty(initialValue: 1);

		_marginForCentering = sizesObservable
			.Select(GetMarginsForCentering)
			.ToReadOnlyReactiveProperty(default);

		InsetSize = sizesObservable
			.Select(GetInsetSize)
			.ToReadOnlyReactiveProperty(default);

		CanvasContentSize = sizesObservable
			.Select(GetCanvasContentSize)
			.ToReadOnlyReactiveProperty(default);
		
		MaxScrollX = sizesObservable
			.Select(x => Math.Max(0, GetCanvasContentSize(x).Width - x.CanvasSize.Width))
			.ToReadOnlyReactiveProperty(default);

		MaxScrollY = sizesObservable
			.Select(x => Math.Max(0, GetCanvasContentSize(x).Height - x.CanvasSize.Height))
			.ToReadOnlyReactiveProperty(default);
	}

	private void BeforeDraw((SKCanvas Canvas, SKPaint? Paint) args)
	{
		SKCanvas canvas = args.Canvas;
		SKSizeI canvasSize = canvas.DeviceClipBounds.Size;
		
		_canvasSize.Value = new SKSizeI(
			(int)(canvasSize.Width / _screenDensity),
			(int)(canvasSize.Height / _screenDensity));
		
		Matrix3x2 matrix = GetTransformationMatrix();

		canvas.Save();
		canvas.Transform(matrix);
	}

	private void AfterDraw((SKCanvas Canvas, SKPaint? paint) args)
	{
		args.Canvas.Restore();
	}

	public void Redraw()
	{
		RedrawRequested?.Invoke(this, EventArgs.Empty);
	}

	public void Draw(SKCanvas canvas, SKPaint? paint = null)
	{
		DrawCommand.Execute((canvas, paint));
	}
	
	public void ResetTransformation()
	{
		ScrollX.Value = 0;
		ScrollY.Value = 0;

		Scale.Value = 1;
	}
	
	public SKPoint GetContentPosition(SKPoint canvasPosition)
	{
		float canvasX = canvasPosition.X * _screenDensity;
		float canvasY = canvasPosition.Y * _screenDensity;

		Matrix3x2 transformationMatrix = GetTransformationMatrix();
		SKPoint contentPosition = transformationMatrix.Invert().Map(canvasX, canvasY);

		return contentPosition;
	}
	
	public SKPoint GetCanvasPosition(SKPoint contentPosition)
	{
		Matrix3x2 transformationMatrix = GetTransformationMatrix();
		SKPoint canvasPosition = transformationMatrix.Map(contentPosition.X, contentPosition.Y);

		return new SKPoint(
			canvasPosition.X * _screenDensity,
			canvasPosition.Y * _screenDensity);
	}

	public void SetScreenDensity(float screenDensity)
	{
		_screenDensity = screenDensity;
	}
	
	public Matrix3x2 GetTransformationMatrix()
	{
		SKSize insetSize = GetInsetSize((ContentSizeScaled.Value, _canvasSize.Value, Scale.Value, InsetRatio.Value));

		float insetHorizontalOffset = insetSize.Width / 2;
		float insetVerticalOffset = insetSize.Height / 2;

		return Matrix3x2.Identity
			.Multiply(Matrix3x2.CreateScale(GlobalContentScale.Value, GlobalContentScale.Value))
			.Multiply(Matrix3x2.CreateScale(_canvasContentRatio.Value, _canvasContentRatio.Value))
			.Multiply(Matrix3x2.CreateTranslation(insetHorizontalOffset, insetVerticalOffset))
			.Multiply(Matrix3x2.CreateScale(Scale.Value, Scale.Value))
			.Multiply(Matrix3x2.CreateTranslation(-ScrollX.Value, -ScrollY.Value))
			.Multiply(Matrix3x2.CreateTranslation(_marginForCentering.Value.X, _marginForCentering.Value.Y))
			.Multiply(Matrix3x2.CreateScale(_screenDensity, _screenDensity));
	}

	private SKPoint GetMarginsForCentering((SKSize ContentSize, SKSizeI CanvasSize, float Scale, float InsetRatio) x)
	{
		SKSizeI canvasSize = x.CanvasSize;
		
		SKSize canvasContentSize = GetCanvasContentSize(x);

		float horizontalMargin = canvasContentSize.Width > canvasSize.Width
			? 0
			: (canvasSize.Width - canvasContentSize.Width) / 2;

		float verticalMargin = canvasContentSize.Height > canvasSize.Height
			? 0
			: (canvasSize.Height - canvasContentSize.Height) / 2;

		return new SKPoint(horizontalMargin, verticalMargin);
	}

	private SKSize GetCanvasContentSize((SKSize ContentSize, SKSizeI CanvasSize, float Scale, float InsetRatio) x)
	{
		float canvasContentRatio = GetCanvasContentRatio(x);
		SKSize contentSize = x.ContentSize;
		
		float scale = x.Scale;

		float contentWidth = contentSize.Width * canvasContentRatio;
		float contentHeight = contentSize.Height * canvasContentRatio;

		SKSize insetSize = GetInsetSize(x);

		if (FitMode == CanvasFitMode.FixedWidth)
		{
			contentWidth = x.CanvasSize.Width / scale;
		}

		if (FitMode == CanvasFitMode.FixedHeight)
		{
			contentHeight = x.CanvasSize.Height / scale;
		}

		float width = contentWidth + insetSize.Width;
		float height = contentHeight + insetSize.Height;

		return new SKSize(width * scale, height * scale);
	}

	private SKSize GetInsetSize((SKSize ContentSize, SKSizeI CanvasSize, float Scale, float InsetRatio) x)
	{
		float canvasContentRatio = GetCanvasContentRatio(x);

		float insetWidth = x.CanvasSize.Width * x.InsetRatio;
		float insetHeight = x.CanvasSize.Height * x.InsetRatio;

		insetWidth = Math.Max(insetWidth, insetHeight);
		insetHeight = Math.Max(insetWidth, insetHeight);

		SKSize contentSize = new(
			x.ContentSize.Width * canvasContentRatio,
			x.ContentSize.Height * canvasContentRatio);

		float widthDifference = x.CanvasSize.Width - contentSize.Width;
		float heightDifference = x.CanvasSize.Height - contentSize.Height;
		
		if (widthDifference >= insetWidth)
		{
			insetWidth = 0;
		}
		else if (widthDifference > 0)
		{
			insetWidth -= widthDifference;
		}

		if (heightDifference >= insetHeight)
		{
			insetHeight = 0;
		}
		else if (heightDifference > 0)
		{
			insetHeight -= heightDifference;
		}
		
		// We want the inset size to be a constant. Divide it by scale to normalize it.
		insetWidth /= x.Scale;
		insetHeight /= x.Scale;

		return new SKSize(insetWidth, insetHeight);
	}

	private float GetCanvasContentRatio((SKSize ContentSize, SKSizeI CanvasSize, float Scale, float InsetRatio) sizes)
	{
		return FitMode switch
		{
			CanvasFitMode.ScaleToFitWidth  => sizes.CanvasSize.Width / sizes.ContentSize.Width,
			CanvasFitMode.FixedWidth       => 1,
			CanvasFitMode.ScaleToFitHeight => sizes.CanvasSize.Height / sizes.ContentSize.Height,
			CanvasFitMode.FixedHeight      => 1,
			_ => throw new ArgumentOutOfRangeException()
		};
	}
}