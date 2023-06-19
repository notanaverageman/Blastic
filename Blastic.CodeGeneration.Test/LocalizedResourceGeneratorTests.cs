using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Blastic.CodeGeneration.Test
{
	public class LocalizedResourceGeneratorTests
	{
		public static void Main()
		{
			string source = GenerateSource(
				"../../../../Blastic.Forms.Sample/Properties/Resources.resx",
				"../../../../Blastic.Forms.Sample/Properties/Resources.tr-tr.resx");

			if (!string.IsNullOrEmpty(source))
			{
				Console.WriteLine(source);
			}
		}

		private static string GenerateSource(params string[] resxPaths)
		{
			List<MetadataReference> references = new();
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

			foreach (Assembly assembly in assemblies)
			{
				if (!assembly.IsDynamic)
				{
					references.Add(MetadataReference.CreateFromFile(assembly.Location));
				}
			}

			List<SyntaxTree> syntaxTrees = new();
			List<AdditionalText> additionalTexts = new();

			foreach (string resxPath in resxPaths)
			{
				AdditionalText additionalText = new ResxAdditionalText(resxPath);
				additionalTexts.Add(additionalText);
			}

			syntaxTrees.Add(CSharpSyntaxTree.ParseText("""
				[Blastic.CodeGeneration.ResxLocalizationSource("Resources.resx")]
				[Blastic.CodeGeneration.ResxLocalizationSource("Resources.tr-tr.resx")]
				public partial class Strings
				{
				}

				[Blastic.CodeGeneration.ResxLocalizableProperties("Resources.resx")]
				[Blastic.CodeGeneration.ResxLocalizableProperties("Resources.tr-tr.resx")]
				public partial class Localization
				{
				}
				"""));


			syntaxTrees.Add(CSharpSyntaxTree.ParseText("[assembly:Blastic.CodeGeneration.CreateLocalizationSource(\"Test\", className: \"Source\")]"));
			syntaxTrees.Add(CSharpSyntaxTree.ParseText("[assembly:Blastic.CodeGeneration.CreateLocalizableProperties(\"Test\")]"));

			CSharpCompilation compilation = CSharpCompilation.Create(
				"original",
				syntaxTrees,
				references,
				new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

			GeneratorDriver driver = CSharpGeneratorDriver.Create(
				new ResxLocalizationSourceGenerator(),
				new ResxLocalizablePropertiesGenerator());

			driver = driver.AddAdditionalTexts(ImmutableArray.CreateRange(additionalTexts));
			
			driver.RunGeneratorsAndUpdateCompilation(
				compilation,
				out Compilation outputCompilation,
				out ImmutableArray<Diagnostic> diagnostics);

			bool hasError = false;

			foreach (Diagnostic diagnostic in diagnostics.Where(x => x.Severity == DiagnosticSeverity.Error))
			{
				hasError = true;
				Console.WriteLine(diagnostic.GetMessage());
			}

			return hasError
				? null
				: string.Join("\r\n", outputCompilation.SyntaxTrees);
		}
	}
}