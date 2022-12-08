using Blastic.Ordering;
using System;

namespace Blastic.ViewManagement.TypeMappers;

/// <summary>
/// A type mapper that return <see cref="TView"/> if the given viewmodel type is <see cref="TViewModel"/>
/// </summary>
/// <typeparam name="TViewModel">Type of the viewmodel</typeparam>
/// <typeparam name="TView">Type of the view</typeparam>
public class DirectTypeMapper<TViewModel, TView> : ITypeMapper
{
	/// <inheritdoc />
	public Order Order { get; }

	/// <summary>
	/// Create a new instance of <see cref="DirectTypeMapper{TViewModel,TView}"/>.
	/// </summary>
	/// <param name="order">Optional order.</param>
	public DirectTypeMapper(Order? order = null)
	{
		Order = order ?? new Order();
	}

	/// <inheritdoc />
	public Type? Map(Type type)
	{
		return type == typeof(TViewModel)
			? typeof(TView)
			: null;
	}
}