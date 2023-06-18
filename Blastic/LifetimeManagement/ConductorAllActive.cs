using System;
using System.Threading;
using Blastic.LifetimeManagement.Contexts;
using DynamicData;

namespace Blastic.LifetimeManagement
{
	/// <summary>
	/// A class that can have many items and any one of them can be active at any time.
	/// </summary>
	/// <typeparam name="T">A type with a lifetime.</typeparam>
	public class ConductorAllActive<T> : ConductorBase<T> where T : IHasLifetime
	{
		/// <summary>
		/// Activate the given item. The item is added to the children it it is not added before.
		/// </summary>
		/// <param name="item">Item to activate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		public void Activate(T item, CancellationToken cancellationToken = default)
		{
			if (!Lifetime.IsActive.Value)
			{
				return;
			}

			if (!Items.Contains(item))
			{
				throw new ArgumentException("Item is not a child of this object.");
			}

			item.Lifetime.Activate(cancellationToken);
		}

		/// <summary>
		/// Deactivate the given item. The item is added to the children it it is not added before.
		/// </summary>
		/// <param name="item">Item to deactivate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		public void Deactivate(T item, CancellationToken cancellationToken = default)
		{
			if (!Items.Contains(item))
			{
				throw new ArgumentException("Item is not a child of this object.");
			}

			item.Lifetime.Deactivate(cancellationToken);
		}

		public override void Close(
			T item,
			bool result = false,
			CancellationToken cancellationToken = default)
		{
			if (!Items.Contains(item))
			{
				throw new ArgumentException("Item is not a child of this object.");
			}

			ClosureContext context = new(result);
			item.Lifetime.Close(cancellationToken, context);

			ItemsSource.Remove(item);
		}
	}
}