using System.Linq;

namespace Blastic.Common
{
	public class Version : Order
	{
		public Version(params int[] numbers) : base(numbers)
		{
		}

		public new static Version Parse(string versionString)
		{
			return new Version(Order.Parse(versionString).Numbers.ToArray());
		}
	}
}