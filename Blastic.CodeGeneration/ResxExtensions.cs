using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Blastic.CodeGeneration
{
	public static class ResxExtensions
	{
		public static List<LocalizedText> GetLocalizedTexts(this GeneratorExecutionContext context)
		{
			List<LocalizedText> localizedTexts = new List<LocalizedText>();

			foreach (AdditionalText resx in context.AdditionalFiles)
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

					LocalizedText localizedText = new LocalizedText(
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
			this GeneratorExecutionContext context,
			List<LocalizedText> localizedTexts,
			StringBuilder builder)
		{
			List<string> designerFilePaths = localizedTexts
				.Select(x => x.Path.Replace(".resx", ".Designer.cs"))
				.Select(Path.GetFullPath)
				.ToList();

			StringComparison pathComparison = Environment.OSVersion.Platform == PlatformID.Unix ||
			                                  Environment.OSVersion.Platform == PlatformID.MacOSX
				? StringComparison.Ordinal
				: StringComparison.OrdinalIgnoreCase;

			SyntaxTree? designerSyntaxTree = context.Compilation.SyntaxTrees
				.Where(x => File.Exists(x.FilePath))
				.FirstOrDefault(
					x =>
					{
						string fullPath = Path.GetFullPath(x.FilePath);

						return designerFilePaths.Any(y => y.Equals(fullPath, pathComparison));
					});

			NamespaceDeclarationSyntax? namespaceDeclaration = designerSyntaxTree?.GetRoot()
				.DescendantNodesAndSelf()
				.OfType<NamespaceDeclarationSyntax>()
				.FirstOrDefault();

			if (namespaceDeclaration == null)
			{
				return;
			}

			builder.Replace("\r\n", "\r\n    ");

			builder.Insert(0, "{\r\n    ");
			builder.Insert(0, $"namespace {namespaceDeclaration.Name}\r\n");

			builder.AppendLine();
			builder.AppendLine("}");
		}
	}
}