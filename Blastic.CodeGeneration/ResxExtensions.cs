using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;

namespace Blastic.CodeGeneration
{
	public static class ResxExtensions
	{
		public static List<LocalizedText> GetLocalizedTexts(
			this ImmutableArray<AdditionalText> texts,
			IReadOnlyList<string> resources)
		{
			List<LocalizedText> localizedTexts = [];

			foreach (AdditionalText resx in texts)
			{
				string resxPath = resx.Path;

				if (!resxPath.EndsWith(".resx", StringComparison.InvariantCultureIgnoreCase))
				{
					continue;
				}

				if (resources.All(x => !resxPath.EndsWith(x)))
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

				string fileName = Path.GetFileNameWithoutExtension(resxPath);
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
						resxPath);

					localizedTexts.Add(localizedText);
				}
			}

			return localizedTexts;
		}
	}
}