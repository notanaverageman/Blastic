using System;

namespace Blastic.ViewManagement.TypeMappers
{
	public class InheritanceTypeMapper : ITypeMapper
	{
		private readonly Type _baseType;
		private readonly Type _output;

		public InheritanceTypeMapper(Type baseType, Type output)
		{
			_baseType = baseType;
			_output = output;
		}

		public Type? Map(Type type)
		{
			return _baseType.IsAssignableFrom(type) ? _output : null;
		}
	}
}