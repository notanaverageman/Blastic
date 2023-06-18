using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Blastic.CodeGeneration
{
	[Generator]
	public class LocalizablePropertiesGenerator : IIncrementalGenerator
	{
		private const string LocalizationService = "Blastic.Services.Localization.ILocalizationService";
		private const string ReadOnlyReactiveProperty = "Blastic.Reactive.IReadOnlyReactiveProperty<string>";
		private const string LocalizableReactiveProperty = "Blastic.Reactive.LocalizableReactiveProperty";

		private const string AttributeNamespace = "Blastic.CodeGeneration";
		private const string AttributeName = "CreateLocalizablePropertiesAttribute";
		private const string AttributeFullName = $"{AttributeNamespace}.{AttributeName}";
		private const string AttributeText = $@"
namespace {AttributeNamespace}
{{
	[System.AttributeUsage(System.AttributeTargets.Assembly)]
	internal class {AttributeName} : System.Attribute
	{{
		public string Namespace {{ get; }}
		public string ClassName {{ get; }}

		public {AttributeName}(string @namespace, string className = ""LocalizableProperties"")
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

		public static void Execute(
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
			Tree<string> tree = BuildTree(localizedTexts, className!);

			StringBuilder code = GenerateClass(tree.Root, 0);

			code.WrapWithNamespace(@namespace!);
			code.Insert(0, "#nullable enable\r\n\r\n");

			string sourceText = code.ToString();

			context.AddSource(className!, sourceText);
		}

		private static StringBuilder GenerateClass(Tree<string>.Node node, int indentation)
		{
			string id = node.Id;
			string className = (node.HasParent ? id + "Texts" : id).ToClassName();

			string baseClass = node.HasValue ? LocalizableReactiveProperty : "System.IDisposable";

			StringBuilder classBuilder = new();
			StringBuilder constructorBuilder = new();
			StringBuilder fieldBuilder = new();
			StringBuilder propertyBuilder = new();
			StringBuilder disposeBuilder = new();

			classBuilder.Indent(indentation).AppendLine($"public partial class {className} : {baseClass}");
			classBuilder.Indent(indentation).AppendLine("{");

			if (node.HasChildren)
			{
				classBuilder.Indent(indentation + 1).AppendLine($"private readonly {LocalizationService} _localizationService;");
				classBuilder.Indent(indentation + 1).AppendLine();
			}

			constructorBuilder.Indent(indentation + 1).AppendLine($"public {className}({LocalizationService} localizationService)");

			if (node.HasValue)
			{
				constructorBuilder.Indent(indentation + 2).AppendLine($": base(localizationService, \"{GetKey(node)}\")");
			}

			constructorBuilder.Indent(indentation + 1).AppendLine("{");
			constructorBuilder.Indent(indentation + 2).AppendLine("_localizationService = localizationService;");
			constructorBuilder.Indent(indentation + 1).AppendLine("}");

			if (node.HasValue)
			{
				disposeBuilder.Indent(indentation + 1).AppendLine("public override void Dispose()");
			}
			else
			{
				disposeBuilder.Indent(indentation + 1).AppendLine("public void Dispose()");
			}

			disposeBuilder.Indent(indentation + 1).AppendLine("{");

			if (node.HasValue)
			{
				disposeBuilder.Indent(indentation + 2).AppendLine("base.Dispose();");
				disposeBuilder.Indent(indentation + 2).AppendLine();
			}

			foreach (Tree<string>.Node child in node.Children)
			{
				if (!child.HasChildren)
				{
					GenerateProperty(fieldBuilder, propertyBuilder, disposeBuilder, child, indentation);
					continue;
				}

				string childId = child.Id;
				string childClass = childId.ToPropertyName() + "Texts";

				fieldBuilder.Indent(indentation + 1).Append($"private {childClass}? ");
				fieldBuilder.AppendLine($"{childId.ToFieldName()};");

				propertyBuilder.Indent(indentation + 1).Append($"public {childClass} ");
				propertyBuilder.Append(childId.ToPropertyName());
				propertyBuilder.Append(" => ");
				propertyBuilder.Append($"{childId.ToFieldName()} ?? ({childId.ToFieldName()} ");
				propertyBuilder.AppendLine($" = new {childClass}(_localizationService));");

				disposeBuilder.Indent(indentation + 2).AppendLine($"{childId.ToFieldName()}?.Dispose();");
			}

			classBuilder.Append(fieldBuilder);
			classBuilder.AppendLine();

			classBuilder.Append(propertyBuilder);
			classBuilder.AppendLine();

			if (node.HasParent)
			{
				classBuilder.Append(constructorBuilder);
				classBuilder.AppendLine();
			}

			disposeBuilder.Indent(indentation + 1).AppendLine("}");
			classBuilder.Append(disposeBuilder);

			foreach (Tree<string>.Node child in node.Children)
			{
				if (!child.HasChildren)
				{
					continue;
				}

				classBuilder.AppendLine();
				StringBuilder childCode = GenerateClass(child, indentation + 1);
				classBuilder.Append(childCode);
			}

			classBuilder.Indent(indentation).AppendLine("}");

			return classBuilder;
		}

		private static void GenerateProperty(
			StringBuilder fieldBuilder,
			StringBuilder propertyBuilder,
			StringBuilder disposeBuilder,
			Tree<string>.Node node,
			int indentation)
		{
			string id = node.Id;
			string key = GetKey(node);

			fieldBuilder.Indent(indentation + 1).Append($"private {LocalizableReactiveProperty}? ");
			fieldBuilder.AppendLine($"{id.ToFieldName()};");

			propertyBuilder.Indent(indentation + 1).Append($"public {ReadOnlyReactiveProperty} ");
			propertyBuilder.Append(id.ToPropertyName());
			propertyBuilder.Append(" => ");
			propertyBuilder.Append($"{id.ToFieldName()} ?? ({id.ToFieldName()} ");
			propertyBuilder.AppendLine($" = new {LocalizableReactiveProperty}(_localizationService, @\"{key}\"));");

			disposeBuilder.Indent(indentation + 2).AppendLine($"{id.ToFieldName()}?.Dispose();");
		}

		private static string GetKey(Tree<string>.Node node)
		{
			List<string> tokens = new();
			Tree<string>.Node? nodeIterator = node;

			while (nodeIterator != null)
			{
				tokens.Add(nodeIterator.Id);
				nodeIterator = nodeIterator.Parent;
			}

			// This is the name of the generated class.
			tokens.RemoveAt(tokens.Count - 1);
			tokens.Reverse();

			return string.Join(".", tokens);
		}

		private static Tree<string> BuildTree(List<LocalizedText> localizedTexts, string className)
		{
			Tree<string> tree = new(className);

			foreach (string id in localizedTexts.Select(x => x.Id).OrderBy(x => x.Length).Distinct())
			{
				string[] tokens = id.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
				Tree<string>.Node node = tree.Root;

				for (int i = 0; i < tokens.Length; i++)
				{
					string token = tokens[i];
					Tree<string>.Node? matchedNode = node.Children.FirstOrDefault(x => x.Id == token);

					if (matchedNode == null)
					{
						bool hasValue = i == tokens.Length - 1;
						matchedNode = node.AddChild(token, hasValue);
					}

					node = matchedNode;
				}
			}

			return tree;
		}
	}
}
