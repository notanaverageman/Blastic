using System.Text;

namespace Blastic.CodeGeneration
{
	public static class StringBuilderExtensions
	{
		public static StringBuilder Indent(this StringBuilder builder, int level)
		{
			builder.Append(' ', level * 4);
			return builder;
		}
	}
}