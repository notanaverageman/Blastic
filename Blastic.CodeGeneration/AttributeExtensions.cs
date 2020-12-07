using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Blastic.CodeGeneration
{
	public static class AttributeExtensions
	{
		public static TAttribute GetAttribute<TAttribute>(this Assembly assembly) where TAttribute : Attribute
		{
			return (TAttribute)Attribute.GetCustomAttribute(assembly, typeof(TAttribute));
		}

		public static IAssemblySymbol AddAssemblyAttribute(
			this GeneratorExecutionContext context,
			string attributeName,
			string defaultClassName)
		{
			string attribute = $@"
namespace Blastic.CodeGeneration
{{
	[System.AttributeUsage(System.AttributeTargets.Assembly)]
	internal class {attributeName} : System.Attribute
	{{
		public string Namespace {{ get; }}
		public string ClassName {{ get; }}

		public {attributeName}(string @namespace, string className = ""{defaultClassName}"")
		{{
			Namespace = @namespace;
			ClassName = className;
		}}
	}}
}}
";

			SourceText sourceText = SourceText.From(attribute.Trim(), Encoding.UTF8);
			context.AddSource(attributeName, sourceText);

			// https://github.com/dotnet/roslyn/issues/49753
			CSharpParseOptions? options = (context.Compilation as CSharpCompilation)
				?.SyntaxTrees[0]
				.Options as CSharpParseOptions;

			Compilation compilation = context.Compilation
				.AddSyntaxTrees(CSharpSyntaxTree.ParseText(sourceText, options));

			return compilation.Assembly;
		}

		public static (string? Namespace, string? ClassName) GetNamespaceAndClassName(
			this IAssemblySymbol assembly,
			string attributeName)
		{
			AttributeData? attributeData = assembly
				.GetAttributes()
				.FirstOrDefault(x => x.AttributeClass?.ToString() == $"Blastic.CodeGeneration.{attributeName}");

			if (attributeData == null)
			{
				return (null, null);
			}

			ImmutableArray<TypedConstant> attributeArguments = attributeData.ConstructorArguments;

			string? @namespace = attributeArguments.FirstOrDefault().Value as string;
			string? className = attributeArguments.Skip(1).FirstOrDefault().Value as string;

			return (@namespace, className);
		}
	}
}