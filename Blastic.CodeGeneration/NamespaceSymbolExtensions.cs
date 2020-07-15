using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Blastic.CodeGeneration
{
	public static class NamespaceSymbolExtensions
	{
		public static IEnumerable<INamedTypeSymbol> GetAllTypes(this INamespaceSymbol namespaceSymbol)
		{
			return namespaceSymbol
				.GetTypeMembers()
				.Concat(namespaceSymbol
					.GetNamespaceMembers()
					.SelectMany(GetAllTypes));
		}
	}
}