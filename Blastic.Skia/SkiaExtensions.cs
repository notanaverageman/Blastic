using SkiaSharp;
using System.Numerics;

namespace Blastic.Skia;

public static class SkiaExtensions
{
	public static SKMatrix ToSkia(this Matrix3x2 matrix)
	{
		return new SKMatrix(
			matrix.M11,
			matrix.M21,
			matrix.M31,
			matrix.M12,
			matrix.M22,
			matrix.M32,
			0,
			0,
			1);
	}

	public static void Transform(this SKCanvas canvas, Matrix3x2 matrix)
	{
		canvas.SetMatrix(canvas.TotalMatrix.PreConcat(matrix.ToSkia()));
	}
}