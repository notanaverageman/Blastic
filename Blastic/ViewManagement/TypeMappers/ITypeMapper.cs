using System;

namespace Blastic.ViewManagement.TypeMappers
{
	public interface ITypeMapper
	{
		Type Map(Type type);
	}
}