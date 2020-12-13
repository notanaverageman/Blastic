using System;
using Blastic.Ordering;

namespace Blastic.ViewManagement.TypeMappers
{
	/// <summary>
	/// A type mapper that return its view type if the given viewmodel type is the same
	/// as or inherits from its viewmodel type.
	/// </summary>
	public class InheritanceTypeMapper : ITypeMapper
	{
		private readonly Type _baseType;
		private readonly Type _output;

		/// <inheritdoc />
		public Order Order { get; }

		/// <summary>
		/// Create a new instance of <see cref="InheritanceTypeMapper"/>.
		/// </summary>
		/// <param name="baseType">Base viewmodel type.</param>
		/// <param name="output">View type.</param>
		/// <param name="order">Optional order.</param>
		public InheritanceTypeMapper(Type baseType, Type output, Order? order = null)
		{
			_baseType = baseType;
			_output = output;

			Order = order ?? new Order();
		}

		/// <inheritdoc />
		public Type? Map(Type type)
		{
			return _baseType.IsAssignableFrom(type) ? _output : null;
		}
	}
}