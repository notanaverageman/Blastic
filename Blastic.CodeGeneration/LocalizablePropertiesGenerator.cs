using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Blastic.CodeGeneration
{
	[Generator]
	public class LocalizablePropertiesGenerator : ISourceGenerator
	{
		private const string AttributeName = "CreateLocalizablePropertiesAttribute";

		public void Initialize(GeneratorInitializationContext context)
		{
		}

		public void Execute(GeneratorExecutionContext context)
		{
			IAssemblySymbol assembly = context.AddAssemblyAttribute(AttributeName, "LocalizableProperties");
			(string? @namespace, string? className) = assembly.GetNamespaceAndClassName(AttributeName);

			context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("ASD1", "Debug", $"{@namespace}, {className}", "Debug", DiagnosticSeverity.Warning, true), null));

			if (string.IsNullOrEmpty(@namespace) || string.IsNullOrEmpty(className))
			{
				return;
			}

			const string localizationService = "Blastic.Services.Localization.ILocalizationService";
			const string readOnlyReactiveProperty = "Blastic.Reactive.IReadOnlyReactiveProperty<string>";
			const string localizableReactiveProperty = "Blastic.Reactive.LocalizableReactiveProperty";

			List<LocalizedText> localizedTexts = context.GetLocalizedTexts();

			StringBuilder classBuilder = new StringBuilder();
			classBuilder.AppendLine($"public partial class {className} : System.IDisposable");
			classBuilder.AppendLine("{");

			classBuilder.Indent(1).AppendLine($"private readonly {localizationService} _localizationService;");
			classBuilder.Indent(1).AppendLine();
			classBuilder.Indent(1).AppendLine($"public {className}({localizationService} localizationService)");
			classBuilder.Indent(1).AppendLine("{");
			classBuilder.Indent(2).AppendLine("_localizationService = localizationService;");
			classBuilder.Indent(1).AppendLine("}");
			classBuilder.Indent(1).AppendLine();

			StringBuilder fieldBuilder = new StringBuilder();
			StringBuilder propertyBuilder = new StringBuilder();
			StringBuilder disposeBuilder = new StringBuilder();

			disposeBuilder.Indent(1).AppendLine("public void Dispose()");
			disposeBuilder.Indent(1).AppendLine("{");

			foreach (IGrouping<string, LocalizedText> grouping in localizedTexts.GroupBy(x => x.Id))
			{
				string id = grouping.Key;

				fieldBuilder.Indent(1).Append($"private {localizableReactiveProperty} ");
				fieldBuilder.AppendLine($"{id.ToFieldName()};");

				propertyBuilder.Indent(1).Append($"public {readOnlyReactiveProperty} ");
				propertyBuilder.Append(id.ToPropertyName());
				propertyBuilder.Append(" => ");
				propertyBuilder.Append($"{id.ToFieldName()} ?? ({id.ToFieldName()} ");
				propertyBuilder.AppendLine($" = new {localizableReactiveProperty}(_localizationService, @\"{id}\"));");

				disposeBuilder.Indent(2).AppendLine($"{id.ToFieldName()}?.Dispose();");
			}

			classBuilder.Append(fieldBuilder);
			classBuilder.AppendLine();

			classBuilder.Append(propertyBuilder);
			classBuilder.AppendLine();

			disposeBuilder.Indent(1).AppendLine("}");
			classBuilder.Append(disposeBuilder);

			classBuilder.Append("}");

			classBuilder.WrapWithNamespace(@namespace!);
			string source = classBuilder.ToString();

			context.AddSource(className!, source);
		}
	}
}
