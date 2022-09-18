using System.Globalization;

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
}