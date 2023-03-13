using SkiaSharp;

namespace Blastic.Maui.Sample;

public static class SkiaExtensions
{
	public static SKImage CreateImage(this SKPicture picture, int imageSize = 64)
	{
		float scale = imageSize / Math.Max(picture.CullRect.Width, picture.CullRect.Height);

		return SKImage.FromPicture(
			picture,
			new SKSizeI(imageSize, imageSize),
			SKMatrix.CreateScale(scale, scale));
	}

	public static void DrawPictureCentered(this SKCanvas canvas, SKPicture picture, float scale = 1)
	{
		using (new SKAutoCanvasRestore(canvas))
		{
			float pictureScale = GetScale(picture);
			scale *= pictureScale;

			canvas.Translate(-picture.CullRect.MidX * scale, -picture.CullRect.MidY * scale);
			canvas.Scale(scale);
			canvas.DrawPicture(picture);
		}
	}

	private static float GetScale(SKPicture picture)
	{
		float width = picture.CullRect.Width;
		float height = picture.CullRect.Height;

		float max = MathF.Max(width, height);
		return 0.5f / max;
	}
}