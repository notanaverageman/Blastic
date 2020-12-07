using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Blastic.CodeGeneration
{
	[Generator]
	public class LocalizationSourceGenerator : ISourceGenerator
	{
		public void Initialize(GeneratorInitializationContext context)
		{
		}

		public void Execute(GeneratorExecutionContext context)
		{
			List<LocalizedText> localizedTexts = context.GetLocalizedTexts();

			StringBuilder classBuilder = new StringBuilder();
			classBuilder.AppendLine("public partial class LocalizationSource : Blastic.Services.Localization.ILocalizationSource");
			classBuilder.AppendLine("{");

			classBuilder.Indent(1).AppendLine("public Blastic.Ordering.Order Order { get; }");
			classBuilder.Indent(1).AppendLine();
			classBuilder.Indent(1).AppendLine("public LocalizationSource(Blastic.Ordering.Order order = null)");
			classBuilder.Indent(1).AppendLine("{");
			classBuilder.Indent(2).AppendLine("Order = order ?? new Blastic.Ordering.Order();");
			classBuilder.Indent(1).AppendLine("}");
			classBuilder.Indent(1).AppendLine();

			StringBuilder methodBuilder = new StringBuilder();
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

				foreach (LocalizedText localizedText in grouping)
				{
					methodBuilder.Indent(3).Append("case ");
					methodBuilder.Append(id.ToPropertyName());
					methodBuilder.Append(" when cultureId == \"");
					methodBuilder.Append(localizedText.Culture);
					methodBuilder.AppendLine("\":");

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
			classBuilder.Append("}");

			context.WrapWithNamespace(localizedTexts, classBuilder);
			string source = classBuilder.ToString();

			context.AddSource("LocalizedResources", source);
		}
	}
}
