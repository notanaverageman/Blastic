using System;

namespace Blastic.Animations
{
	/// <summary>
	/// Static class that contains various easing functions.
	/// </summary>
	public static class Easing
	{
		public static double SineIn(double x)
		{
			return 1 - Math.Cos(x * Math.PI / 2);
		}

		public static double SineOut(double x)
		{
			return Math.Sin(x * Math.PI / 2);
		}

		public static double SineInOut(double x)
		{
			return (1 - Math.Cos(x * Math.PI)) / 2;
		}

		public static double QuadraticIn(double x)
		{
			return x * x;
		}

		public static double QuadraticOut(double x)
		{
			return x * (2 - x);
		}

		public static double QuadraticInOut(double x)
		{
			if (x < 0.5)
			{
				return 2 * x * x;
			}

			return -2 * x * (x - 2) - 1;
		}

		public static double CubicIn(double x)
		{
			return x * x * x;
		}

		public static double CubicOut(double x)
		{
			double a = x - 1;
			double a3 = a * a * a;

			return a3 + 1;
		}

		public static double CubicInOut(double x)
		{
			if (x < 0.5)
			{
				return 4 * x * x * x;
			}

			double a = 2 * (x - 1);
			double a3 = a * a * a;

			return a3 / 2 + 1;
		}

		public static double QuarticIn(double x)
		{
			return x * x * x * x;
		}

		public static double QuarticOut(double x)
		{
			double a = x - 1;
			double a2 = a * a;
			double a4 = a2 * a2;

			return 1 - a4;
		}

		public static double QuarticInOut(double x)
		{
			if (x < 0.5)
			{
				double x2 = x * x;
				double x4 = x2 * x2;

				return 8 * x4;
			}

			double a = x - 1;
			double a2 = a * a;
			double a4 = a2 * a2;

			return -8 * a4 + 1;
		}

		public static double QuinticIn(double x)
		{
			return x * x * x * x * x;
		}

		public static double QuinticOut(double x)
		{
			double a = x - 1;
			double a2 = a * a;
			double a5 = a2 * a2 * a;

			return a5 + 1;
		}

		public static double QuinticInOut(double x)
		{
			if (x < 0.5)
			{
				double x2 = x * x;
				double x5 = x2 * x2 * x;

				return 16 * x5;
			}

			double a = x - 1;
			double a2 = a * a;
			double a5 = a2 * a2 * a;

			return 16 * a5 + 1;
		}

		public static double ExponentialIn(double x)
		{
			return x == 0
				? x
				: Math.Pow(2, 10 * (x - 1));
		}

		public static double ExponentialOut(double x)
		{
			return x == 1
				? x
				: 1 - Math.Pow(2, -10 * x);
		}

		public static double ExponentialInOut(double x)
		{
			if (x < 0.5)
			{
				return Math.Pow(2, 20 * x - 10) / 2;
			}

			return Math.Pow(2, -20 * x + 10) / -2 + 1;
		}

		public static double CircularIn(double x)
		{
			return 1 - Math.Sqrt(1 - x * x);
		}

		public static double CircularOut(double x)
		{
			return Math.Sqrt((2 - x) * x);
		}

		public static double CircularInOut(double x)
		{
			if (x < 0.5)
			{
				return (1 - Math.Sqrt(1 - 4 * x * x)) / 2;
			}

			double a = 2 * x;

			return (Math.Sqrt((3 - a) * (a - 1)) + 1) / 2;
		}

		public static double BackIn(double x)
		{
			return x * (x * x - Math.Sin(x * Math.PI));
		}

		public static double BackOut(double x)
		{
			double a = 1 - x;

			return 1 - a * (a * a - Math.Sin(a * Math.PI));
		}

		public static double BackInOut(double x)
		{
			if (x < 0.5)
			{
				double a = 2 * x;
				return a * (a * a - Math.Sin(a * Math.PI)) / 2;
			}

			double b = 2 * (1 - x);
			return 1 - b * (b * b - Math.Sin(b * Math.PI)) / 2;
		}

		public static double ElasticIn(double x)
		{
			return Math.Sin(13 * Math.PI / 2 * x) * Math.Pow(2, 10 * (x - 1));
		}

		public static double ElasticOut(double x)
		{
			return Math.Sin(-13 * Math.PI / 2 * (x + 1)) * Math.Pow(2, -10 * x) + 1;
		}

		public static double ElasticInOut(double x)
		{
			double a = 2 * x;

			if (x < 0.5)
			{
				return Math.Sin(13 * Math.PI / 2 * a) * Math.Pow(2, 10 * (a - 1)) / 2;
			}

			return (Math.Sin(-13 * Math.PI / 2 * a) * Math.Pow(2, -10 * (a - 1)) + 2) / 2;
		}

		public static double BounceIn(double x)
		{
			return 1 - BounceOut(1 - x);
		}

		public static double BounceOut(double x)
		{
			return x switch
			{
				< 4 / 11.0 => 121 * x * x / 16.0,
				< 8 / 11.0 => 363 / 40.0 * x * x - 99 / 10.0 * x + 17 / 5.0,
				< 9 / 10.0 => 4356 / 361.0 * x * x - 35442 / 1805.0 * x + 16061 / 1805.0,
				_ => 54 / 5.0 * x * x - 513 / 25.0 * x + 268 / 25.0
			};
		}

		public static double BounceInOut(double x)
		{
			if (x < 0.5)
			{
				return (1 - BounceOut(1 - 2 * x)) / 2;
			}

			return (BounceOut(2 * x - 1) + 1) / 2;
		}
	}
}