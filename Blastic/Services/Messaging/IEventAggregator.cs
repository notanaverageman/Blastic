using System;

namespace Blastic.Services.Messaging
{
	/// <summary>
	/// A message broker that publishes messages to subscribers.
	/// </summary>
	public interface IEventAggregator : IDisposable
	{
		/// <summary>
		/// Get observable for a message type.
		/// </summary>
		/// <typeparam name="T">Type of the message.</typeparam>
		/// <returns>An observable for message type.</returns>
		IObservable<T> GetEventBus<T>();

		/// <summary>
		/// Register an action to be called whenever a message is published.
		/// </summary>
		/// <typeparam name="T">Type of the message.</typeparam>
		/// <param name="action">Action to execute.</param>
		/// <returns>A disposable that ends the subscription upon disposal.</returns>
		IDisposable Subscribe<T>(Action<T> action);

		/// <summary>
		/// Register an action to be called on UI thread whenever a message is published.
		/// </summary>
		/// <typeparam name="T">Type of the message.</typeparam>
		/// <param name="action">Action to execute.</param>
		/// <returns>A disposable that ends the subscription upon disposal.</returns>
		IDisposable SubscribeOnUIThread<T>(Action<T> action);

		/// <summary>
		/// Publish a new message to subscribers.
		/// </summary>
		/// <typeparam name="T">Type of the message.</typeparam>
		/// <param name="event">The message.</param>
		void Publish<T>(T @event);
	}
}