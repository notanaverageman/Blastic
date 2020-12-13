using System;
using Blastic.Ordering;

namespace Blastic.ViewManagement.TypeMappers
{
	/// <summary>
	/// A type mapper that return its view type if the given viewmodel type's name equals
	/// to the view's type name + a suffix. This class is used for mapping values such as:
	/// <c>SomeViewModel -> SomeView</c>
	/// </summary>
	public class SuffixTypeMapper : ITypeMapper
	{
		private readonly string _viewSuffix;
		private readonly string _viewModelSuffix;

		/// <inheritdoc />
		public Order Order { get; }

		public SuffixTypeMapper(
			string viewSuffix,
			string viewModelSuffix,
			Order? order = null)
		{
			_viewSuffix = viewSuffix;
			_viewModelSuffix = viewModelSuffix;

			Order = order ?? new Order();
		}

		/// <inheritdoc />
		public Type? Map(Type type)
		{
			string? typeName = type.AssemblyQualifiedName;

			if (typeName == null)
			{
				return null;
			}

			typeName = NormalizeTypeName(typeName);

			int indexOfViewModelSuffix = typeName.IndexOf(_viewModelSuffix);

			if (indexOfViewModelSuffix < 0)
			{
				return null;
			}

			string prefix = typeName.Substring(0, indexOfViewModelSuffix);
			string suffix = typeName.Substring(indexOfViewModelSuffix + _viewModelSuffix.Length);

			string viewTypeName = $"{prefix}{_viewSuffix}{suffix}";

			return Type.GetType(viewTypeName);
		}

		private string NormalizeTypeName(string typeName)
		{
			int indexOfBacktick = typeName.IndexOf('`');
			int indexOfBracketAndComma = typeName.IndexOf("]],");
			int indexOfComma = typeName.IndexOf(',');

			if (indexOfBacktick < 0)
			{
				return typeName;
			}

			string prefix = typeName.Substring(0, indexOfBacktick);
			string suffix = indexOfBracketAndComma > 0
				? typeName.Substring(indexOfBracketAndComma + 2) // Discard ]]
				: typeName.Substring(indexOfComma);

			return prefix + suffix;
		}
	}
}