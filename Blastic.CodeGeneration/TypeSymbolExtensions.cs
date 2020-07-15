using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Blastic.DynamicControls.Attributes;
using Microsoft.CodeAnalysis;

namespace Blastic.CodeGeneration
{
	public static class TypeSymbolExtensions
	{
		private static INamedTypeSymbol? _mappingAttributeSymbol;

		private static INamedTypeSymbol GetGeneratorMappingAttributeSymbol(Compilation compilation)
		{
			return _mappingAttributeSymbol ??= compilation.GetTypeByMetadataName(typeof(LabelAttribute).FullName)!;
		}

		public static bool IsInterface(this ITypeSymbol typeSymbol)
		{
			return typeSymbol.TypeKind == TypeKind.Interface;
		}

		public static IEnumerable<INamedTypeSymbol> GetAllInterfaces(this ITypeSymbol typeSymbol, bool includeCurrent = true)
		{
			if (includeCurrent && typeSymbol.IsInterface())
			{
				yield return (INamedTypeSymbol)typeSymbol;
			}

			ITypeSymbol? currentTypeSymbol = typeSymbol;

			do
			{
				ImmutableArray<INamedTypeSymbol> interfaces = currentTypeSymbol.Interfaces;
				foreach (INamedTypeSymbol @interface in interfaces)
				{
					yield return @interface;

					foreach (INamedTypeSymbol innerInterface in @interface.GetAllInterfaces())
					{
						yield return innerInterface;
					}
				}

				currentTypeSymbol = currentTypeSymbol.BaseType;

			} while (currentTypeSymbol != null && currentTypeSymbol.SpecialType != SpecialType.System_Object);
		}

		//public static bool Contains(this ITypeSymbol type, IMethodSymbol method)
		//{
		//	return type
		//		.GetMembers()
		//		.OfType<IMethodSymbol>()
		//		.Any(typeMethod => typeMethod.SameAs(method));
		//}

		//public static bool ContainsExtension(this ITypeSymbol type, IMethodSymbol method)
		//{
		//	return type
		//		.GetMembers()
		//		.OfType<IMethodSymbol>()
		//		.Any(
		//			typeMethod =>
		//				typeMethod.IsExtensionMethod &&
		//				typeMethod.SameAs(method, 1));
		//}

		public static ImmutableArray<(string, string, bool)> GetGenericMappings(this ITypeSymbol type, Compilation compilation)
		{
			INamedTypeSymbol mappingAttributeSymbol = GetGeneratorMappingAttributeSymbol(compilation);
			IEnumerable<(string, string, bool)> mappings = type.GetAttributes()
				.Where(attribute => SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, mappingAttributeSymbol))
				.Select(attribute => (
					(string)attribute.ConstructorArguments[0].Value!,
					(string)attribute.ConstructorArguments[1].Value!,
					(bool)attribute.ConstructorArguments[2].Value!));

			return ImmutableArray.CreateRange(mappings);
		}

		public static string ToDisplayString(this ITypeSymbol type, ImmutableArray<(string, string, bool)> genericsMapping)
		{
			string result = type.ToDisplayString();

			foreach ((string from, string to, bool _) in genericsMapping.Reverse())
			{
				result = result.Replace(from, to);
			}

			return result;
		}

		//public static string ToDisplayString(this ITypeSymbol type, ITypeSymbol enumerableType, ITypeSymbol enumeratorType, ImmutableArray<(string, string, bool)> genericsMapping)
		//{
		//	if (type is INamedTypeSymbol namedType && namedType.IsGenericType)
		//	{
		//		string displayName = namedType.ToDisplayString();
		//		string classDisplayName = displayName.Substring(0, displayName.IndexOf('<'));

		//		if (namedType.GetAllInterfaces()
		//			.Any(@interface =>
		//				@interface.Name == "IReadOnlyList" ||
		//				@interface.Name == "IValueEnumerable" ||
		//				@interface.Name == "IAsyncValueEnumerable"))
		//		{
		//			return $"{classDisplayName}{namedType.TypeArguments.AsTypeArgumentsString(enumerableType, enumeratorType, genericsMapping)}";
		//		}
		//	}

		//	return type.ToDisplayString(genericsMapping);
		//}

		public static IEnumerable<(string Name, ITypeParameterSymbol TypeParameter)> ExtractTypeArgumentsFromType(ITypeSymbol typeSymbol, HashSet<string> set, ImmutableArray<(string, string, bool)> genericsMapping)
		{
			if (typeSymbol is INamedTypeSymbol namedType && namedType.IsGenericType)
			{
				ImmutableArray<ITypeSymbol> typeArguments = namedType.TypeArguments;

				foreach (ITypeSymbol typeArgument in typeArguments)
				{
					if (!(typeArgument is ITypeParameterSymbol typeParameter))
					{
						continue;
					}

					(bool isMapped, string mappedTo, bool isMappedToType) = typeParameter.IsMapped(genericsMapping);

					if (isMappedToType)
					{
						continue;
					}

					string displayString = typeParameter.ToDisplayString();

					if (!set.Add(displayString))
					{
						continue;
					}

					yield return (displayString, typeParameter);

					if (isMapped && set.Add(mappedTo))
					{
						yield return (mappedTo, typeParameter);
					}
				}
			}
		}

		//public static IEnumerable<(string Name, IEnumerable<string> Constraints)> ExtractMethodTypeArguments(this ITypeSymbol extendedType, IMethodSymbol method, ImmutableArray<(string, string, bool)> genericsMapping)
		//{
		//	HashSet<string> set = new HashSet<string>();

		//	// get the type arguments list from the extension method parameters
		//	List<(string Name, ITypeParameterSymbol TypeParameter)> parameteresTypeArguments =
		//		ExtractTypeArgumentsFromType(extendedType, set, genericsMapping)
		//		.Concat(method.Parameters.SelectMany(parameter => ExtractTypeArgumentsFromType(parameter.Type, set, genericsMapping)))
		//		.ToList();

		//	// get the type arguments list from the contraints
		//	List<(string Name, ITypeParameterSymbol TypeParameter)> constraintsTypeArguments = parameteresTypeArguments
		//		.SelectMany(typeArgument => typeArgument.TypeParameter.ConstraintTypes)
		//		.SelectMany(type => ExtractTypeArgumentsFromType(type, set, genericsMapping))
		//		.ToList();

		//	return parameteresTypeArguments.Concat(constraintsTypeArguments)
		//		.Select(typeArgument => (typeArgument.Name, typeArgument.TypeParameter.AsConstraintsStrings()));
		//}

		private static (bool, string, bool) IsMapped(this ITypeSymbol type, ImmutableArray<(string, string, bool)> genericsMapping)
		{
			foreach ((string from, string to, bool isType) in genericsMapping)
			{
				if (type.Name == from)
				{
					return (true, to, isType);
				}
			}
			return default;
		}

		//public static string AsTypeArgumentsString(this ImmutableArray<ITypeSymbol> typeParameters, ITypeSymbol enumerableType, ITypeSymbol enumeratorType, ImmutableArray<(string, string, bool)> genericsMapping)
		//{
		//	List<string> result = new List<string>();

		//	foreach (ITypeSymbol t in typeParameters)
		//	{
		//		switch (t.Name)
		//		{
		//			case "TEnumerable":
		//			case "TList":
		//				result.Add(enumerableType.ToDisplayString());
		//				break;
		//			case "TEnumerator":
		//				if (enumeratorType != null)
		//				{
		//					result.Add(enumeratorType.ToDisplayString());
		//				}
		//				break;
		//			default:
		//				result.Add(t.ToDisplayString(genericsMapping));
		//				break;
		//		}
		//	}

		//	return (result.Count == 0)
		//		? string.Empty
		//		: $"<{result.ToCommaSeparated()}>";
		//}
	}
}