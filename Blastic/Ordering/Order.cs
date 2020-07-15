using System;
using System.Collections.Generic;
using System.Linq;

namespace Blastic.Ordering
{
	public class Order : IEquatable<Order>, IComparable<Order>, IComparable
	{
		public static readonly Order AbsoluteMinimum = new Order(true, false);
		public static readonly Order AbsoluteMaximum = new Order(false, true);

		private readonly bool _isAbsoluteMinimum;
		private readonly bool _isAbsoluteMaximum;

		private readonly List<int> _numbers;

		public IReadOnlyList<int> Numbers => _numbers;

		public Order(params int[] numbers)
		{
			_numbers = new List<int>(numbers);
		}

		private Order(bool isAbsoluteMinimum, bool isAbsoluteMaximum)
		{
			_isAbsoluteMinimum = isAbsoluteMinimum;
			_isAbsoluteMaximum = isAbsoluteMaximum;

			_numbers = new List<int>(0);
		}

		public bool Equals(Order? other)
		{
			return CompareTo(other) == 0;
		}

		public override bool Equals(object? obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((Order)obj);
		}

		public override int GetHashCode()
		{
			return _numbers.GetHashCode();
		}

		public static bool operator ==(Order? left, Order? right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Order? left, Order? right)
		{
			return !Equals(left, right);
		}

		public int CompareTo(Order? other)
		{
			if (other == null)
			{
				return 1;
			}

			if (_isAbsoluteMinimum)
			{
				return other._isAbsoluteMinimum ? 0 : -1;
			}

			if (_isAbsoluteMaximum)
			{
				return other._isAbsoluteMaximum ? 0 : 1;
			}

			if (other._isAbsoluteMinimum)
			{
				return _isAbsoluteMinimum ? 0 : 1;
			}

			if (other._isAbsoluteMaximum)
			{
				return _isAbsoluteMaximum ? 0 : -1;
			}

			int maxCount = _numbers.Count > other._numbers.Count
				? _numbers.Count
				: other._numbers.Count;

			for (int i = 0; i < maxCount; i++)
			{
				int left = i < _numbers.Count
					? _numbers[i]
					: 0;

				int right = i < other._numbers.Count
					? other._numbers[i]
					: 0;

				if (left < right)
				{
					return -1;
				}

				if (left > right)
				{
					return 1;
				}
			}

			return 0;
		}

		public int CompareTo(object? obj)
		{
			if (ReferenceEquals(null, obj)) return 1;
			if (ReferenceEquals(this, obj)) return 0;

			return obj is Order other
				? CompareTo(other)
				: throw new ArgumentException($"Object must be of type {nameof(Order)}");
		}

		public static bool operator <(Order? left, Order? right)
		{
			return Comparer<Order?>.Default.Compare(left, right) < 0;
		}

		public static bool operator >(Order? left, Order? right)
		{
			return Comparer<Order?>.Default.Compare(left, right) > 0;
		}

		public static bool operator <=(Order? left, Order? right)
		{
			return Comparer<Order?>.Default.Compare(left, right) <= 0;
		}

		public static bool operator >=(Order? left, Order? right)
		{
			return Comparer<Order?>.Default.Compare(left, right) >= 0;
		}

		public override string ToString()
		{
			if (_isAbsoluteMinimum)
			{
				return "Absolute Minimum";
			}

			if (_isAbsoluteMaximum)
			{
				return "Absolute Maximum";
			}

			if (!Numbers.Any())
			{
				return "0";
			}

			return string.Join(".", Numbers);
		}

		public static Order Parse(string orderString)
		{
			if (orderString == "")
			{
				return new Order();
			}

			int[] numbers = orderString
				.Split('.')
				.Select(int.Parse)
				.ToArray();

			return new Order(numbers);
		}
	}
}