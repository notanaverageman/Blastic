using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Blastic.CodeGeneration
{
	public static class RoslynExtensions
	{
		public static MethodDeclarationSyntax WithModifiers(
			this MethodDeclarationSyntax syntax,
			params SyntaxKind[] modifiers)
		{
			return syntax.WithModifiers(TokenList(modifiers.Select(Token)));
		}

		public static MethodDeclarationSyntax WithBody(
			this MethodDeclarationSyntax syntax,
			params StatementSyntax[] statements)
		{
			return syntax.WithBody(Block(List(statements)));
		}

		public static MethodDeclarationSyntax WithBody(
			this MethodDeclarationSyntax syntax,
			IEnumerable<StatementSyntax> statements)
		{
			return syntax.WithBody(Block(List(statements)));
		}

		public static MethodDeclarationSyntax PublicMethod(Type returnType, string name)
		{
			return MethodDeclaration(
					IdentifierName(returnType.Name),
					Identifier(name))
				.WithModifiers(SyntaxKind.PublicKeyword);
		}

		public static MethodDeclarationSyntax PublicMethod(TypeSyntax returnType, string name)
		{
			return MethodDeclaration(
					returnType,
					Identifier(name))
				.WithModifiers(SyntaxKind.PublicKeyword);
		}

		public static LocalDeclarationStatementSyntax Variable(Type type, string name)
		{
			return LocalDeclarationStatement(
				VariableDeclaration(GenericName(Identifier(name))
						.WithTypeArgumentList(
							TypeArgumentList(
								SingletonSeparatedList<TypeSyntax>(IdentifierName(type.Name)))))
					.WithVariables(
						SingletonSeparatedList(
							VariableDeclarator(
									Identifier(name))
								.WithInitializer(
									EqualsValueClause(
										ObjectCreationExpression(
												GenericName(
														Identifier(type.Name!))
													.WithTypeArgumentList(
														TypeArgumentList(
															SingletonSeparatedList<TypeSyntax>(IdentifierName(type.Name!)))))
											.WithArgumentList(
												ArgumentList()))))));
		}
	}
}