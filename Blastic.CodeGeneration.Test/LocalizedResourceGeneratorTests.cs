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
			List<MetadataReference> references = new List<MetadataReference>();
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

			foreach (Assembly assembly in assemblies)
			{
				if (!assembly.IsDynamic)
				{
					references.Add(MetadataReference.CreateFromFile(assembly.Location));
				}
			}

			List<SyntaxTree> syntaxTrees = new List<SyntaxTree>();
			List<AdditionalText> additionalTexts = new List<AdditionalText>();

			foreach (string resxPath in resxPaths)
			{
				AdditionalText additionalText = new ResxAdditionalText(resxPath);
				additionalTexts.Add(additionalText);
			}
			
			syntaxTrees.Add(CSharpSyntaxTree.ParseText("[assembly:Blastic.CodeGeneration.CreateLocalizationSource(\"Test\", className: \"Source\")]"));
			syntaxTrees.Add(CSharpSyntaxTree.ParseText("[assembly:Blastic.CodeGeneration.CreateLocalizableProperties(\"Test\")]"));

			CSharpCompilation compilation = CSharpCompilation.Create(
				"original",
				syntaxTrees,
				references,
				new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

			GeneratorDriver driver = CSharpGeneratorDriver.Create(
				new LocalizationSourceGenerator(),
				new LocalizablePropertiesGenerator());

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