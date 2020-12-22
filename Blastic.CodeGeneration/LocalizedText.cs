using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Blastic.CodeGeneration
{
	public class LocalizedText
	{
		public string Id { get; }
		public string Text { get; }
		public string Culture { get; }
		public string Path { get; }

		public LocalizedText(
			string id,
			string text,
			string culture,
			string path)
		{
			Id = id;
			Text = text;
			Culture = new CultureInfo(culture).Name.ToLowerInvariant();
			Path = path;
		}
	}

	public static class StringExtensions
	{
		public static string ToVariableName(this string s)
		{
			return Regex.Replace(s, "[^_a-zA-Z0-9]", "");
		}

		public static string ToFieldName(this string s)
		{
			string variableName = s.ToVariableName();

			if (char.IsUpper(variableName.First()))
			{
				variableName = char.ToLowerInvariant(variableName.First()) + variableName.Substring(1);
			}

			if (!variableName.StartsWith("_"))
			{
				variableName = "_" + variableName;
			}

			return variableName;
		}

		public static string ToPropertyName(this string s)
		{
			string variableName = s.ToVariableName();

			if (char.IsLower(variableName.First()))
			{
				variableName = char.ToUpperInvariant(variableName.First()) + variableName.Substring(1);
			}

			if (char.IsDigit(variableName.First()))
			{
				variableName = "_" + variableName;
			}

			return variableName;
		}

		public static string ToClassName(this string s)
		{
			return s.ToPropertyName();
		}
	}
}