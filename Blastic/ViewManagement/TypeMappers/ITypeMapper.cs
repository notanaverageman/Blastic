using System;
using Blastic.Ordering;

namespace Blastic.ViewManagement.TypeMappers
{
	public interface ITypeMapper
	{
		Order Order { get; }
		Type? Map(Type type);
	}
}