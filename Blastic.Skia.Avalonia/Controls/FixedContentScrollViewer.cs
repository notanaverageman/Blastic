using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;

namespace Blastic.Skia.Avalonia.Controls;

public class FixedContentScrollViewer : ScrollViewer, IStyleable
{
	Type IStyleable.StyleKey => typeof(ScrollViewer);

	public static readonly AvaloniaProperty<Control?> ContentToFixProperty =
		AvaloniaProperty.RegisterDirect<FixedContentScrollViewer, Control?>(
			nameof(ContentToFix),
			x => x.ContentToFix,
			(x, y) => x.ContentToFix = y);

	private Control? _contentToFix;
	
	public Control? ContentToFix
	{
		get => _contentToFix;
		set => SetAndRaise(ContentToFixProperty, ref _contentToFix, value);
	}

	protected override void OnScrollChanged(ScrollChangedEventArgs e)
	{
		if (_contentToFix == null)
		{
			return;
		}

		if (_contentToFix.RenderTransform is not TranslateTransform transform)
		{
			transform = new TranslateTransform();
			_contentToFix.RenderTransform = transform;
		}

		transform.X += e.OffsetDelta.X;
		transform.Y += e.OffsetDelta.Y;

		base.OnScrollChanged(e);
	}
}