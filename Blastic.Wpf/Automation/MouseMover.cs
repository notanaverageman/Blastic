using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Blastic.ViewManagement;
using Point = System.Drawing.Point;

namespace Blastic.Wpf.Automation
{
	public static partial class AutomationExtensions
	{
		public static async Task MoveMouseTo(this IViewAware viewAware, object bindingSource, double speed = 1.5)
		{
			FrameworkElement element = viewAware.GetView(bindingSource);

			if (element == null)
			{
				return;
			}

			System.Windows.Point point = element.PointToScreen(new System.Windows.Point(
				element.ActualWidth / 2,
				element.ActualHeight / 2));

			await MoveMouseTo((int)point.X, (int)point.Y, speed);
		}

		public static async Task MoveMouseTo(int x, int y, double speed = 1.5)
		{
			await Task.Run(() => MouseMover.MoveMouse(x, y, speed));
		}

		private static class MouseMover
		{
			[DllImport("user32.dll")]
			private static extern bool SetCursorPos(int x, int y);

			[DllImport("user32.dll")]
			private static extern bool GetCursorPos(out Point p);

			private static readonly Random Random = new Random();

			public static void MoveMouse(int x, int y, double speed)
			{
				GetCursorPos(out Point current);

				WindMouse(
					current.X,
					current.Y,
					x,
					y,
					9.0,
					3.0,
					10.0 / speed,
					15.0 / speed,
					10.0 * speed,
					10.0 * speed);
			}

			private static void WindMouse(
				double currentX,
				double currentY,
				double targetX,
				double targetY,
				double gravity,
				double wind,
				double minWait,
				double maxWait,
				double maxStep,
				double targetArea)
			{
				double windX = 0;
				double windY = 0;
				double veloX = 0;
				double veloY = 0;

				int newX = (int)Math.Round(currentX);
				int newY = (int)Math.Round(currentY);

				double waitDiff = maxWait - minWait;
				double sqrt2 = Math.Sqrt(2.0);
				double sqrt3 = Math.Sqrt(3.0);
				double sqrt5 = Math.Sqrt(5.0);

				double dist = Hypotenuse(targetX - currentX, targetY - currentY);

				while (dist > 1.0)
				{

					wind = Math.Min(wind, dist);

					if (dist >= targetArea)
					{
						int w = Random.Next((int)Math.Round(wind) * 2 + 1);
						windX = windX / sqrt3 + (w - wind) / sqrt5;
						windY = windY / sqrt3 + (w - wind) / sqrt5;
					}
					else
					{
						windX = windX / sqrt2;
						windY = windY / sqrt2;
						if (maxStep < 3)
						{
							maxStep = Random.Next(3) + 3.0;
						}
						else
						{
							maxStep = maxStep / sqrt5;
						}
					}

					veloX += windX;
					veloY += windY;
					veloX = veloX + gravity * (targetX - currentX) / dist;
					veloY = veloY + gravity * (targetY - currentY) / dist;

					if (Hypotenuse(veloX, veloY) > maxStep)
					{
						double randomDist = maxStep / 2.0 + Random.Next((int)Math.Round(maxStep) / 2);
						double veloMag = Hypotenuse(veloX, veloY);
						veloX = (veloX / veloMag) * randomDist;
						veloY = (veloY / veloMag) * randomDist;
					}

					int oldX = (int)Math.Round(currentX);
					int oldY = (int)Math.Round(currentY);

					currentX += veloX;
					currentY += veloY;

					dist = Hypotenuse(targetX - currentX, targetY - currentY);
					newX = (int)Math.Round(currentX);
					newY = (int)Math.Round(currentY);

					if (oldX != newX || oldY != newY)
					{
						SetCursorPos(newX, newY);
					}

					double step = Hypotenuse(currentX - oldX, currentY - oldY);
					int wait = (int)Math.Round(waitDiff * (step / maxStep) + minWait);

					Thread.Sleep(wait);
				}

				int endX = (int)Math.Round(targetX);
				int endY = (int)Math.Round(targetY);

				if (endX != newX || endY != newY)
				{
					SetCursorPos(endX, endY);
				}
			}

			private static double Hypotenuse(double dx, double dy)
			{
				return Math.Sqrt(dx * dx + dy * dy);
			}
		}
	}
}