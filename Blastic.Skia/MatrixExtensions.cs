using SkiaSharp;
using System.Numerics;

namespace Blastic.Skia;

public static class MatrixExtensions
{
	public static Matrix3x2 Multiply(this Matrix3x2 left, Matrix3x2 right)
	{
		return Matrix3x2.Multiply(left, right);
	}

	public static Matrix3x2 Invert(this Matrix3x2 matrix)
	{
		Matrix3x2.Invert(matrix, out Matrix3x2 result);
		return result;
	}

	public static SKPoint Map(this Matrix3x2 matrix, float x, float y)
	{
		return matrix.Map(new SKPoint(x, y));
	}

	public static SKPoint Map(this Matrix3x2 matrix, SKPoint point)
	{
		Matrix3x2 pointMatrix = new(
			1,
			0,
			0,
			1,
			point.X,
			point.Y);

		Matrix3x2 result = Matrix3x2.Multiply(pointMatrix, matrix);

		return new SKPoint(result.M31, result.M32);
	}
}