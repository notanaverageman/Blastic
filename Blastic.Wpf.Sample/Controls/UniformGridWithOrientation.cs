using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Blastic.Wpf.Sample.Controls
{
	public class UniformGridWithOrientation : UniformGrid
	{
		public static readonly DependencyProperty OrientationProperty =
			DependencyProperty.Register(
				nameof(OrientationProperty).Replace("Property", ""),
				typeof(System.Windows.Controls.Orientation),
				typeof(UniformGridWithOrientation),
				new FrameworkPropertyMetadata(
					System.Windows.Controls.Orientation.Vertical,
					FrameworkPropertyMetadataOptions.AffectsMeasure));

		public System.Windows.Controls.Orientation Orientation
		{
			get => (System.Windows.Controls.Orientation)GetValue(OrientationProperty);
			set => SetValue(OrientationProperty, value);
		}

		private int _columns;
		private int _rows;

		protected override Size MeasureOverride(Size constraint)
		{
			UpdateComputedValues();

			Size availableSize = new(constraint.Width / _columns, constraint.Height / _rows);
			double width = 0.0;
			double height = 0.0;

			int index = 0;
			int count = InternalChildren.Count;

			while (index < count)
			{
				UIElement element = InternalChildren[index];
				element.Measure(availableSize);

				Size desiredSize = element.DesiredSize;

				if (width < desiredSize.Width)
				{
					width = desiredSize.Width;
				}

				if (height < desiredSize.Height)
				{
					height = desiredSize.Height;
				}

				index++;
			}

			return new Size(width * _columns, height * _rows);
		}

		private void UpdateComputedValues()
		{
			_columns = Columns;
			_rows = Rows;

			if (FirstColumn >= _columns)
			{
				FirstColumn = 0;
			}

			if (FirstColumn > 0)
			{
				throw new NotImplementedException("There is no support for seting the FirstColumn (nor the FirstRow).");
			}

			if (_rows != 0 && _columns != 0)
			{
				return;
			}

			int visibleChildrenCount = InternalChildren
				.Cast<UIElement>()
				.Count(child => child.Visibility != Visibility.Collapsed);

			if (visibleChildrenCount == 0)
			{
				visibleChildrenCount = 1;
			}

			if (_rows == 0)
			{
				if (_columns > 0)
				{
					_rows = (visibleChildrenCount + FirstColumn + (_columns - 1)) / _columns;
				}
				else
				{
					_rows = (int)Math.Sqrt(visibleChildrenCount);

					if (_rows * _rows < visibleChildrenCount)
					{
						_rows++;
					}

					_columns = _rows;
				}
			}
			else if (_columns == 0)
			{
				_columns = (visibleChildrenCount + (_rows - 1)) / _rows;
			}
		}

		protected override Size ArrangeOverride(Size arrangeSize)
		{
			Rect finalRect = new(0, 0, arrangeSize.Width / _columns, arrangeSize.Height / _rows);

			double height = finalRect.Height;
			double numX = arrangeSize.Height - 1.0;

			finalRect.X += finalRect.Width * FirstColumn;

			foreach (UIElement element in InternalChildren)
			{
				element.Arrange(finalRect);

				if (element.Visibility == Visibility.Collapsed)
				{
					continue;
				}

				finalRect.Y += height;

				if (finalRect.Y >= numX)
				{
					finalRect.X += finalRect.Width;
					finalRect.Y = 0.0;
				}
			}

			return arrangeSize;
		}
	}
}