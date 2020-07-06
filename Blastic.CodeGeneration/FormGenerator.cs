using System.Collections.Generic;
using System.Linq;
using Blastic.DynamicControls;
using Blastic.Reactive;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Blastic.CodeGeneration.RoslynExtensions;

namespace Blastic.CodeGeneration
{
	public class FormGenerator
	{
		public string Generate(ClassDeclarationSyntax classDeclaration)
		{
			List<MemberDeclarationSyntax> members = new List<MemberDeclarationSyntax>();

			List<PropertyDeclarationSyntax> properties = CreateProperties(classDeclaration);
			ConstructorDeclarationSyntax constructor = CreateConstructor(classDeclaration);
			MethodDeclarationSyntax toDynamicModelMethod = CreateToDynamicModelMethod(classDeclaration);
			MethodDeclarationSyntax toTypeMethod = CreateToTypeMethod(classDeclaration);

			members.AddRange(properties);
			members.Add(constructor);
			members.Add(toDynamicModelMethod);
			members.Add(toTypeMethod);

			return ClassDeclaration(GetFormIdentifier(classDeclaration))
				.WithMembers(List(members))
				.WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
				.NormalizeWhitespace()
				.ToFullString();
		}

		private ConstructorDeclarationSyntax CreateConstructor(ClassDeclarationSyntax classDeclaration)
		{
			List<StatementSyntax> statements = new List<StatementSyntax>();

			foreach (PropertyDeclarationSyntax propertyDeclaration in classDeclaration.Members.OfType<PropertyDeclarationSyntax>())
			{
				ExpressionStatementSyntax statement = ExpressionStatement(
					AssignmentExpression(
						SyntaxKind.SimpleAssignmentExpression,
						IdentifierName(propertyDeclaration.Identifier),
						ObjectCreationExpression(
								GenericName(
										Identifier(nameof(ReactiveProperty<object>)))
									.WithTypeArgumentList(
										TypeArgumentList(
											SingletonSeparatedList(propertyDeclaration.Type))))
							.WithArgumentList(
								ArgumentList(
									SingletonSeparatedList(
										Argument(
											MemberAccessExpression(
												SyntaxKind.SimpleMemberAccessExpression,
												IdentifierName("value"),
												IdentifierName(propertyDeclaration.Identifier))))))));

				statements.Add(statement);
			}

			return ConstructorDeclaration(
					Identifier(GetFormIdentifier(classDeclaration)))
				.WithModifiers(
					TokenList(
						Token(SyntaxKind.PublicKeyword)))
				.WithParameterList(
					ParameterList(
						SingletonSeparatedList(
							Parameter(
									Identifier("value"))
								.WithType(
									IdentifierName(classDeclaration.Identifier.Text)))))
				.WithBody(Block(List(statements)));
		}

		private List<PropertyDeclarationSyntax> CreateProperties(ClassDeclarationSyntax classDeclaration)
		{
			List<PropertyDeclarationSyntax> properties = new List<PropertyDeclarationSyntax>();

			foreach (PropertyDeclarationSyntax propertyDeclaration in classDeclaration.Members.OfType<PropertyDeclarationSyntax>())
			{
				PropertyDeclarationSyntax formPropertyDeclaration = PropertyDeclaration(
						GenericName(
								Identifier(nameof(IReactiveProperty)))
							.WithTypeArgumentList(
								TypeArgumentList(
									SingletonSeparatedList(propertyDeclaration.Type))),
						Identifier(propertyDeclaration.Identifier.Text))
					.WithModifiers(
						TokenList(
							Token(SyntaxKind.PublicKeyword)))
					.WithAccessorList(
						AccessorList(
							SingletonList(
								AccessorDeclaration(
										SyntaxKind.GetAccessorDeclaration)
									.WithSemicolonToken(
										Token(SyntaxKind.SemicolonToken)))));

				properties.Add(formPropertyDeclaration);
			}

			return properties;
		}

		private MethodDeclarationSyntax CreateToDynamicModelMethod(ClassDeclarationSyntax classDeclaration)
		{
			List<StatementSyntax> statements = new List<StatementSyntax>();

			LocalDeclarationStatementSyntax modelDeclaration = LocalDeclarationStatement(
					VariableDeclaration(
							IdentifierName(nameof(DynamicModel)))
						.WithVariables(
							SingletonSeparatedList(
								VariableDeclarator(
										Identifier("model"))
									.WithInitializer(
										EqualsValueClause(
											ObjectCreationExpression(
													IdentifierName(nameof(DynamicModel)))
												.WithArgumentList(
													ArgumentList()))))));

			statements.Add(modelDeclaration);

			foreach (PropertyDeclarationSyntax propertyDeclaration in classDeclaration.Members.OfType<PropertyDeclarationSyntax>())
			{
				string methodName;

				switch (propertyDeclaration.Type)
				{
					case PredefinedTypeSyntax predefinedType when
						predefinedType.Keyword.Kind() == SyntaxKind.ShortKeyword ||
						predefinedType.Keyword.Kind() == SyntaxKind.IntKeyword ||
						predefinedType.Keyword.Kind() == SyntaxKind.FloatKeyword ||
						predefinedType.Keyword.Kind() == SyntaxKind.DoubleKeyword:
						methodName = nameof(ElementContainerExtensions.AddNumber);
						break;
					case PredefinedTypeSyntax predefinedType when predefinedType.Keyword.Kind() == SyntaxKind.StringKeyword:
						methodName = nameof(ElementContainerExtensions.AddText);
						break;
					default:
						continue;
				}

				ExpressionStatementSyntax statement = ExpressionStatement(
					InvocationExpression(
							MemberAccessExpression(
								SyntaxKind.SimpleMemberAccessExpression,
								IdentifierName("model"),
								IdentifierName(methodName)))
						.WithArgumentList(
							ArgumentList(
								SingletonSeparatedList(
									Argument(
										IdentifierName(propertyDeclaration.Identifier))))));

				statements.Add(statement);
			}

			ReturnStatementSyntax returnStatement = ReturnStatement(IdentifierName("model"));
			statements.Add(returnStatement);

			return PublicMethod(typeof(DynamicModel), "ToDynamicModel").WithBody(statements);
		}

		private MethodDeclarationSyntax CreateToTypeMethod(ClassDeclarationSyntax classDeclaration)
		{
			List<StatementSyntax> statements = new List<StatementSyntax>();

			LocalDeclarationStatementSyntax valueDeclaration = LocalDeclarationStatement(
					VariableDeclaration(
							IdentifierName(classDeclaration.Identifier))
						.WithVariables(
							SingletonSeparatedList(
								VariableDeclarator(
										Identifier("value"))
									.WithInitializer(
										EqualsValueClause(
											ObjectCreationExpression(
													IdentifierName(classDeclaration.Identifier))
												.WithArgumentList(
													ArgumentList()))))));

			statements.Add(valueDeclaration);

			foreach (PropertyDeclarationSyntax propertyDeclaration in classDeclaration.Members.OfType<PropertyDeclarationSyntax>())
			{
				ExpressionStatementSyntax statement = ExpressionStatement(
					AssignmentExpression(
						SyntaxKind.SimpleAssignmentExpression,
						MemberAccessExpression(
							SyntaxKind.SimpleMemberAccessExpression,
							IdentifierName("value"),
							IdentifierName(propertyDeclaration.Identifier)),
						MemberAccessExpression(
							SyntaxKind.SimpleMemberAccessExpression,
							IdentifierName(propertyDeclaration.Identifier),
							IdentifierName(nameof(IReactiveProperty.Value)))));

				statements.Add(statement);
			}

			ReturnStatementSyntax returnStatement = ReturnStatement(IdentifierName("value"));
			statements.Add(returnStatement);

			return PublicMethod(IdentifierName(classDeclaration.Identifier), "To" + classDeclaration.Identifier.Text)
				.WithBody(statements);
		}

		private string GetFormIdentifier(ClassDeclarationSyntax classDeclaration)
		{
			return classDeclaration.Identifier.Text + "Form";
		}
	}
}