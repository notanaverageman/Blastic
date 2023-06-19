using Blastic.CodeGeneration.CSharp;
using Blastic.CodeGeneration.Utility;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System;

namespace Blastic.CodeGeneration;

public static class CodeBuilderExtensions
{
	public static IDisposable AddNamespace(this CodeBuilder builder, INamedTypeSymbol classSymbol)
	{
		return classSymbol.ContainingNamespace.IsGlobalNamespace
			? Disposable.Empty
			: builder.Namespace(classSymbol.ContainingNamespace.ToDisplayString());
	}

	public static ClassBuilder AddClass(this CodeBuilder builder, INamedTypeSymbol classSymbol)
	{
		string visibility = SyntaxFacts.GetText(classSymbol.DeclaredAccessibility);

		ClassBuilder classBuilder = builder.Class(classSymbol.Name).Partial();
		classBuilder.Visibility(visibility);

		return classBuilder;
	}
}