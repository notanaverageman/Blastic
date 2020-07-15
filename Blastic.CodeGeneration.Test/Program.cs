using Blastic.DynamicControls.Attributes;

namespace Blastic.CodeGeneration.Test
{
	public class Customer
	{
		public int Id { get; set; }

		[Label(nameof(Name))]
		public string Name { get; set; }
		public string Address { get; set; }
	}

	public class Program
	{
		public static void Main()
		{
			HelloWorldGenerated.HelloWorld.SayHello();
		}
	}
}