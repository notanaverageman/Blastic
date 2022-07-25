using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Blastic.CodeGeneration
{
	[Generator]
	public class LocalizationSourceGenerator : IIncrementalGenerator
	{
		private const string AttributeNamespace = "Blastic.CodeGeneration";
		private const string AttributeName = "CreateLocalizationSourceAttribute";
		private const string AttributeFullName = $"{AttributeNamespace}.{AttributeName}";
		private const string AttributeText = $@"
namespace {AttributeNamespace}
{{
	[System.AttributeUsage(System.AttributeTargets.Assembly)]
	internal class {AttributeName} : System.Attribute
	{{
		public string Namespace {{ get; }}
		public string ClassName {{ get; }}

		public {AttributeName}(string @namespace, string className = ""LocalizationSource"")
		{{
			Namespace = @namespace;
			ClassName = className;
		}}
	}}
}}";

		public void Initialize(IncrementalGeneratorInitializationContext context)
		{
			context.RegisterPostInitializationOutput(i => i.AddSource(
				$"{AttributeName}.g.cs",
				AttributeText.Trim()));

			IncrementalValueProvider<ImmutableArray<AdditionalText>> additionalTexts = context.AdditionalTextsProvider
				.Where(x => x.Path.EndsWith(".resx", StringComparison.InvariantCultureIgnoreCase))
				.Where(x => x != null)
				.Collect();

			context.RegisterSourceOutput(
				context.CompilationProvider.Combine(additionalTexts),
				static (context, source) => Execute(context, source.Left, source.Right));
		}

		private static void Execute(
			SourceProductionContext context,
			Compilation compilation,
			ImmutableArray<AdditionalText> source)
		{
			(string? @namespace, string? className) = compilation.GetNamespaceAndClassName(AttributeFullName);

			if (string.IsNullOrEmpty(@namespace) || string.IsNullOrEmpty(className))
			{
				return;
			}

			List<LocalizedText> localizedTexts = source.GetLocalizedTexts();

			StringBuilder classBuilder = new();
			classBuilder.AppendLine($"public partial class {className} : Blastic.Services.Localization.ILocalizationSource");
			classBuilder.AppendLine("{");

			classBuilder.Indent(1).AppendLine("public Blastic.Ordering.Order Order { get; }");
			classBuilder.Indent(1).AppendLine();
			classBuilder.Indent(1).AppendLine($"public {className}(Blastic.Ordering.Order order = null)");
			classBuilder.Indent(1).AppendLine("{");
			classBuilder.Indent(2).AppendLine("Order = order ?? new Blastic.Ordering.Order();");
			classBuilder.Indent(1).AppendLine("}");
			classBuilder.Indent(1).AppendLine();

			StringBuilder methodBuilder = new();
			methodBuilder.Indent(1).AppendLine("public string GetValue(string key, System.Globalization.CultureInfo culture)");
			methodBuilder.Indent(1).AppendLine("{");
			methodBuilder.Indent(2).AppendLine("string cultureId = culture.Name.ToLowerInvariant();");
			methodBuilder.Indent(1).AppendLine();
			methodBuilder.Indent(2).AppendLine("switch(key)");
			methodBuilder.Indent(2).AppendLine("{");

			foreach (IGrouping<string, LocalizedText> grouping in localizedTexts.GroupBy(x => x.Id))
			{
				string id = grouping.Key;

				classBuilder.Indent(1).Append("public const string ");
				classBuilder.Append(id.ToPropertyName());
				classBuilder.Append(" = @\"");
				classBuilder.Append(id);
				classBuilder.AppendLine("\";");

				foreach (LocalizedText localizedText in grouping.OrderByDescending(x => x.Culture.Length))
				{
					methodBuilder.Indent(3).Append("case ");
					methodBuilder.Append(id.ToPropertyName());

					if (localizedText.Culture != "")
					{
						methodBuilder.Append(" when cultureId == \"");
						methodBuilder.Append(localizedText.Culture);
						methodBuilder.Append("\"");
					}

					methodBuilder.AppendLine(":");

					methodBuilder.Indent(4).Append("return @\"");
					methodBuilder.Append(localizedText.Text);
					methodBuilder.AppendLine("\";");
				}
			}

			methodBuilder.Indent(3).AppendLine("default:");
			methodBuilder.Indent(4).AppendLine("return null;");

			methodBuilder.Indent(2).AppendLine("}");
			methodBuilder.Indent(1).AppendLine("}");

			classBuilder.AppendLine();
			classBuilder.Append(methodBuilder);
			classBuilder.AppendLine("}");

			classBuilder.WrapWithNamespace(@namespace!);
			string sourceText = classBuilder.ToString();

			context.AddSource(className!, sourceText);
		}
	}
}
