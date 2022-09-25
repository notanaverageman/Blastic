using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Blastic.Ordering;
using Blastic.Reactive;

namespace Blastic.Commanding;

public interface IReadOnlyCommand
{
	/// <summary>
	/// An observable property that emits when command's CanExecute property changes.
	/// </summary>
	public IReadOnlyReactiveProperty<bool> CanExecuteObservable { get; }

	/// <summary>
	/// Registers the given action to be executed when the <see cref="Command"/> is executed.
	/// </summary>
	/// <param name="action">The action to execute.</param>
	/// <param name="order">Order of the action among other actions.</param>
	/// <returns>An <see cref="IDisposable"/> that unregisters the action when disposed.</returns>
	IDisposable Subscribe(Action action, Order? order = null);
	
	/// <summary>
	/// Registers the given action to be executed when the <see cref="Command"/> is executed.
	/// </summary>
	/// <param name="action">The action to execute.</param>
	/// <param name="order">Order of the action among other actions.</param>
	/// <returns>An <see cref="IDisposable"/> that unregisters the action when disposed.</returns>
	IDisposable Subscribe(Action<CancellationToken> action, Order? order = null);
	
	/// <summary>
	/// Registers the given action to be executed when the <see cref="Command"/> is executed.
	/// </summary>
	/// <param name="action">The action to execute.</param>
	/// <param name="order">Order of the action among other actions.</param>
	/// <returns>An <see cref="IDisposable"/> that unregisters the action when disposed.</returns>
	IDisposable SubscribeFinally(Action action, Order? order = null);
	
	/// <summary>
	/// Registers the given action to be executed after all the normal actions are finished. These actions will
	/// be called even if the execution is cancelled.
	/// </summary>
	/// <param name="action">The action to execute.</param>
	/// <param name="order">Order of the action among other actions.</param>
	/// <returns>An <see cref="IDisposable"/> that unregisters the action when disposed.</returns>
	IDisposable SubscribeFinally(Action<CancellationToken> action, Order? order = null);
	
	TaskAwaiter GetAwaiter();
}

public interface IReadOnlyCommand<T> : IReadOnlyCommand
{
	/// <summary>
	/// Registers the given action to be executed when the <see cref="Command"/> is executed.
	/// </summary>
	/// <param name="action">The action to execute.</param>
	/// <param name="order">Order of the action among other actions.</param>
	/// <returns>An <see cref="IDisposable"/> that unregisters the action when disposed.</returns>
	IDisposable Subscribe(Action<T?> action, Order? order = null);
	
	/// <summary>
	/// Registers the given action to be executed when the <see cref="Command"/> is executed.
	/// </summary>
	/// <param name="action">The action to execute.</param>
	/// <param name="order">Order of the action among other actions.</param>
	/// <returns>An <see cref="IDisposable"/> that unregisters the action when disposed.</returns>
	IDisposable Subscribe(Action<T?, CancellationToken> action, Order? order = null);
	
	/// <summary>
	/// Registers the given action to be executed after all the normal actions are finished. These actions will
	/// be called even if the execution is cancelled.
	/// </summary>
	/// <param name="action">The action to execute.</param>
	/// <param name="order">Order of the action among other actions.</param>
	/// <returns>An <see cref="IDisposable"/> that unregisters the action when disposed.</returns>
	IDisposable SubscribeFinally(Action<T?> action, Order? order = null);
	
	/// <summary>
	/// Registers the given action to be executed after all the normal actions are finished. These actions will
	/// be called even if the execution is cancelled.
	/// </summary>
	/// <param name="action">The action to execute.</param>
	/// <param name="order">Order of the action among other actions.</param>
	/// <returns>An <see cref="IDisposable"/> that unregisters the action when disposed.</returns>
	IDisposable SubscribeFinally(Action<T?, CancellationToken> action, Order? order = null);
}