using System;
using Blastic.Ordering;

namespace Blastic.ViewManagement.TypeMappers
{
	/// <summary>
	/// Maps a viewmodel type to a view type.
	/// </summary>
	public interface ITypeMapper
	{
		/// <summary>
		/// Order of the type mapper among other type mappers.
		/// </summary>
		Order Order { get; }

		/// <summary>
		/// Map the given viewmodel type to a view type.
		/// </summary>
		/// <param name="type">Type of the viewmodel.</param>
		/// <returns>The corresponding view type or null if the type cannot be matched.</returns>
		Type? Map(Type type);
	}
}