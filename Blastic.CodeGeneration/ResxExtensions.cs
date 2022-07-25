using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;

namespace Blastic.CodeGeneration
{
	public static class ResxExtensions
	{
		public static List<LocalizedText> GetLocalizedTexts(this ImmutableArray<AdditionalText> texts)
		{
			List<LocalizedText> localizedTexts = new();

			foreach (AdditionalText resx in texts)
			{
				if (!resx.Path.EndsWith(".resx", StringComparison.InvariantCultureIgnoreCase))
				{
					continue;
				}

				string? sourceText = resx.GetText()?.ToString();

				if (sourceText == null)
				{
					continue;
				}

				XElement document;
				try
				{
					document = XElement.Parse(sourceText);
				}
				catch (XmlException)
				{
					continue;
				}

				string fileName = Path.GetFileNameWithoutExtension(resx.Path);
				string culture = Path.GetExtension(fileName).Trim('.');

				foreach (XElement element in document.Elements("data"))
				{
					string? id = element.Attribute("name")?.Value;
					string? text = element.Attribute("value")?.Value;

					if (string.IsNullOrEmpty(text))
					{
						text = element.Element("value")?.Value;
					}

					if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(text))
					{
						continue;
					}

					LocalizedText localizedText = new(
						id!,
						text!,
						culture,
						resx.Path);
					localizedTexts.Add(localizedText);
				}
			}

			return localizedTexts;
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