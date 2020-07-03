using System;
using Blastic.Ordering;

namespace Blastic.ViewManagement.TypeMappers
{
	public class InheritanceTypeMapper : ITypeMapper
	{
		private readonly Type _baseType;
		private readonly Type _output;

		public Order Order { get; }

		public InheritanceTypeMapper(Type baseType, Type output, Order? order = null)
		{
			_baseType = baseType;
			_output = output;

			Order = order ?? new Order();
		}

		public Type? Map(Type type)
		{
			return _baseType.IsAssignableFrom(type) ? _output : null;
		}
	}
}