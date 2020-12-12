using System.Linq;

namespace Blastic.Ordering
{
	/// <summary>
	/// A class that is basically an order but with another name.
	/// </summary>
	public class Version : Order
	{
		/// <inheritdoc />
		public Version(params int[] numbers) : base(numbers)
		{
		}

		/// <summary>
		/// Parses a string to a <see cref="Version"/> object. String should be
		/// of a format that each integer has a . between them.
		/// </summary>
		/// <param name="versionString">The string to parse.</param>
		/// <returns>The parsed version.</returns>
		public new static Version Parse(string versionString)
		{
			return new Version(Order.Parse(versionString).Numbers.ToArray());
		}
	}
}