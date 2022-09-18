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

		public static void WrapWithNamespace(
			this StringBuilder builder,
			string @namespace)
		{
			builder.Replace("\r\n", "\r\n    ");

			builder.Insert(0, "{\r\n    ");
			builder.Insert(0, $"namespace {@namespace}\r\n");

			builder.Replace("    ", "", builder.Length - 4, 4);

			builder.AppendLine("}");
		}
	}
}