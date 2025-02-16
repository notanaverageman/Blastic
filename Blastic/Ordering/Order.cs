using System;
using System.Collections.Generic;
using System.Linq;

namespace Blastic.Ordering
{
	/// <summary>
	/// A class that can be used to specify order of things in a flexible manner.
	/// </summary>
	/// <remarks>
	/// This object consists of a list of integers. When two <see cref="Order"/> objects
	/// are compared, each integer in their lists are compared according to their indexes.
	/// If one of the lists do not have an integer at an index, it is assumed to have 0.
	/// <para>
	/// For example, if we compare order (1, 2) with (1), first the integers at index 0 are compared.
	/// If they are equal then the integers at index 1 are compared. Since the second order
	/// does not have an integer at that index, it is assumed that it has 0 and the first order
	/// is determined to be greater than the second one.
	/// </para>
	/// <para>
	/// This makes it possible to create an order between any two orders. For example,
	/// given two orders (1, 1), (1, 2), the order (1, 1, 1) is between them.
	/// </para>
	/// <para>
	/// There are two special order objects <see cref="AbsoluteMinimum"/> and <see cref="AbsoluteMaximum"/>.
	/// <see cref="AbsoluteMinimum"/> is always less than (greater than for <see cref="AbsoluteMaximum"/>) other orders and
	/// it is only equal to itself.
	/// </para>
	/// </remarks>
	public class Order : IEquatable<Order>, IComparable<Order>, IComparable
	{
		/// <summary>
		/// The order that is less than every other order.
		/// </summary>
		public static readonly Order AbsoluteMinimum = new(true, false);

		/// <summary>
		/// The order that is greater than every other order.
		/// </summary>
		public static readonly Order AbsoluteMaximum = new(false, true);

		private readonly bool _isAbsoluteMinimum;
		private readonly bool _isAbsoluteMaximum;

		private readonly List<int> _numbers;

		/// <summary>
		/// List of integers in this order.
		/// </summary>
		public IReadOnlyList<int> Numbers => _numbers;

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="numbers"></param>
		public Order(params int[] numbers)
		{
			_numbers = new List<int>(numbers);
		}

		private Order(bool isAbsoluteMinimum, bool isAbsoluteMaximum)
		{
			_isAbsoluteMinimum = isAbsoluteMinimum;
			_isAbsoluteMaximum = isAbsoluteMaximum;

			_numbers = [];
		}

		/// <inheritdoc />
		public bool Equals(Order? other)
		{
			return CompareTo(other) == 0;
		}

		/// <inheritdoc />
		public override bool Equals(object? obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((Order)obj);
		}

		/// <inheritdoc />
		public override int GetHashCode()
		{
			unchecked
			{
				int hash = 19;

				foreach (int number in _numbers)
				{
					hash = hash * 31 + number.GetHashCode();
				}

				return hash;
			}
		}

		public static bool operator ==(Order? left, Order? right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Order? left, Order? right)
		{
			return !Equals(left, right);
		}

		/// <inheritdoc />
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

		/// <inheritdoc />
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

		/// <inheritdoc />
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

		/// <summary>
		/// Parses a string to an <see cref="Order"/> object. String should be
		/// of a format that each integer has a . between them.
		/// </summary>
		/// <param name="orderString">The string to parse.</param>
		/// <returns>The parsed order.</returns>
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