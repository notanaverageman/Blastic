using System;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Ordering;

namespace Blastic.Commanding;

public interface IReadOnlyAsyncCommand : IReadOnlyAsyncCommand<object>
{
}

public interface IReadOnlyAsyncCommand<T> : IReadOnlyCommand<T>
{
	/// <summary>
	/// Registers the given action to be executed when the <see cref="AsyncCommand"/> is executed.
	/// </summary>
	/// <param name="action">The action to execute.</param>
	/// <param name="order">Order of the action among other actions.</param>
	/// <returns>An <see cref="IDisposable"/> that unregisters the action when disposed.</returns>
	IDisposable Subscribe(Func<Task> action, Order? order = null);

	/// <summary>
	/// Registers the given action to be executed when the <see cref="AsyncCommand"/> is executed.
	/// </summary>
	/// <param name="action">The action to execute.</param>
	/// <param name="order">Order of the action among other actions.</param>
	/// <returns>An <see cref="IDisposable"/> that unregisters the action when disposed.</returns>
	IDisposable Subscribe(Func<T?, Task> action, Order? order = null);

	/// <summary>
	/// Registers the given action to be executed when the <see cref="AsyncCommand"/> is executed.
	/// </summary>
	/// <param name="action">The action to execute.</param>
	/// <param name="order">Order of the action among other actions.</param>
	/// <returns>An <see cref="IDisposable"/> that unregisters the action when disposed.</returns>
	IDisposable Subscribe(Func<CancellationToken, Task> action, Order? order = null);

	/// <summary>
	/// Registers the given action to be executed when the <see cref="AsyncCommand"/> is executed.
	/// </summary>
	/// <param name="action">The action to execute.</param>
	/// <param name="order">Order of the action among other actions.</param>
	/// <returns>An <see cref="IDisposable"/> that unregisters the action when disposed.</returns>
	IDisposable Subscribe(Func<T?, CancellationToken, Task> action, Order? order = null);

	/// <summary>
	/// Registers the given action to be executed after all the normal actions are finished. These actions will
	/// be called even if the execution is cancelled.
	/// </summary>
	/// <param name="action">The action to execute.</param>
	/// <param name="order">Order of the action among other actions.</param>
	/// <returns>An <see cref="IDisposable"/> that unregisters the action when disposed.</returns>
	IDisposable SubscribeFinally(Func<Task> action, Order? order = null);

	/// <summary>
	/// Registers the given action to be executed after all the normal actions are finished. These actions will
	/// be called even if the execution is cancelled.
	/// </summary>
	/// <param name="action">The action to execute.</param>
	/// <param name="order">Order of the action among other actions.</param>
	/// <returns>An <see cref="IDisposable"/> that unregisters the action when disposed.</returns>
	IDisposable SubscribeFinally(Func<T?, Task> action, Order? order = null);

	/// <summary>
	/// Registers the given action to be executed after all the normal actions are finished. These actions will
	/// be called even if the execution is cancelled.
	/// </summary>
	/// <param name="action">The action to execute.</param>
	/// <param name="order">Order of the action among other actions.</param>
	/// <returns>An <see cref="IDisposable"/> that unregisters the action when disposed.</returns>
	IDisposable SubscribeFinally(Func<CancellationToken, Task> action, Order? order = null);

	/// <summary>
	/// Registers the given action to be executed after all the normal actions are finished. These actions will
	/// be called even if the execution is cancelled.
	/// </summary>
	/// <param name="action">The action to execute.</param>
	/// <param name="order">Order of the action among other actions.</param>
	/// <returns>An <see cref="IDisposable"/> that unregisters the action when disposed.</returns>
	IDisposable SubscribeFinally(Func<T?, CancellationToken, Task> action, Order? order = null);
}