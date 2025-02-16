using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Commanding.Concurrency;
using Blastic.Commanding.ErrorHandling;
using Blastic.Ordering;
using Blastic.Platform;
using Blastic.Reactive;
using UnhandledExceptionEventArgs = Blastic.Commanding.ErrorHandling.UnhandledExceptionEventArgs;

namespace Blastic.Commanding
{
	/// <inheritdoc cref="AsyncCommand{T}"/>
	public class AsyncCommand : AsyncCommand<object?>, IReadOnlyAsyncCommand
	{
		/// <summary>
		/// The default order of the actions if their order is not specified.
		/// </summary>
		public static readonly Order DefaultOrder = new();

		/// <inheritdoc />
		public AsyncCommand() : this((IObservable<bool>?)null)
		{
		}

		/// <inheritdoc />
		public AsyncCommand(IObservable<bool>? canExecute) : base(canExecute)
		{
		}

		/// <inheritdoc />
		public AsyncCommand(Action action) : this()
		{
			Subscribe(action);
		}

		/// <inheritdoc />
		public AsyncCommand(Action<CancellationToken> action) : this()
		{
			Subscribe(action);
		}

		/// <inheritdoc />
		public AsyncCommand(Func<Task> action) : this()
		{
			Subscribe(action);
		}

		/// <inheritdoc />
		public AsyncCommand(Func<CancellationToken, Task> action) : this()
		{
			Subscribe(action);
		}

		/// <inheritdoc />
		public AsyncCommand(IObservable<bool>? canExecute, Action action) : this(canExecute)
		{
			Subscribe(action);
		}

		/// <inheritdoc />
		public AsyncCommand(IObservable<bool>? canExecute, Action<CancellationToken> action) : this(canExecute)
		{
			Subscribe(action);
		}

		/// <inheritdoc />
		public AsyncCommand(IObservable<bool>? canExecute, Func<Task> action) : this(canExecute)
		{
			Subscribe(action);
		}

		/// <inheritdoc />
		public AsyncCommand(IObservable<bool>? canExecute, Func<CancellationToken, Task> action) : this(canExecute)
		{
			Subscribe(action);
		}

		/// <inheritdoc cref="AsyncCommand{T}.Execute(T, CancellationToken)" />
		public async Task Execute(CancellationToken cancellationToken = default)
		{
			await Execute(null, cancellationToken);
		}
	}

	/// <summary>
	/// An implementation of <see cref="ICommand"/> with reactive patterns.
	/// </summary>
	/// <remarks>
	/// <para>
	/// A <see cref="AsyncCommand"/> can hold a number of actions that can be ordered with
	/// <see cref="Order"/> classes. When the command is executed, these actions will
	/// be executed based on their order. Actions with lesser order will be executed
	/// first and actions with the same order will be executed concurrently.
	/// </para>
	/// <para>
	/// Command's state can be observed via <see cref="CanExecuteObservable"/> and <see cref="IsExecuting"/>.
	/// Reentrancy behavior can be specified by setting <see cref="ReentrancyHandler"/>.
	/// </para>
	/// <para>
	/// You can register an action via the constructor or the <see cref="Subscribe(Action, Order?)"/> methods.
	/// <see cref="Subscribe(Action, Order?)"/> methods return an <see cref="IDisposable"/> that unregisters the
	/// action when disposed.
	/// </para>
	/// </remarks>
	/// <typeparam name="T"></typeparam>
	public class AsyncCommand<T> : ICommand, IReadOnlyAsyncCommand<T>, IObserver<bool>
	{
		public event EventHandler<UnhandledExceptionEventArgs>? UnhandledException;

		private readonly List<OrderedAction> _actions;
		private readonly List<OrderedAction> _finallyActions;
		private readonly IReactiveProperty<bool> _isExecuting;

		private IReadOnlyReactiveProperty<bool> _canExecuteObservable;
		private IDisposable? _canExecuteSubscription;

		private TaskCompletionSource<bool>? _awaitableTask;

		/// <inheritdoc />
		public event EventHandler? CanExecuteChanged;

		/// <summary>
		/// An observable property that emits when command's CanExecute property changes.
		/// </summary>
		public IReadOnlyReactiveProperty<bool> CanExecuteObservable
		{
			get => _canExecuteObservable;
			set
			{
				_canExecuteSubscription?.Dispose();
				_canExecuteObservable = value;

				_canExecuteSubscription = _canExecuteObservable
					.ObserveOnUI()
					.Subscribe(this);
			}
		}

		/// <summary>
		/// An observable property that emits true when the <see cref="AsyncCommand"/> starts execution and
		/// emits false when the <see cref="AsyncCommand"/> finishes the execution.
		/// </summary>
		public IReadOnlyReactiveProperty<bool> IsExecuting => _isExecuting;

		/// <summary>
		/// Property that defines the behavior of the command when it is executed concurrently.
		/// </summary>
		public IReentrancyHandler ReentrancyHandler { get; set; }

		/// <summary>
		/// Default constructor that creates an always executable <see cref="AsyncCommand"/>.
		/// </summary>
		public AsyncCommand() : this((IObservable<bool>?)null)
		{
		}

		/// <summary>
		/// Creates a <see cref="AsyncCommand"/> with given observable for can execute property.
		/// </summary>
		/// <param name="canExecute">An observable that determines the can execute property. Can execute will always be true if this parameter is null.</param>
		public AsyncCommand(IObservable<bool>? canExecute)
		{
			_actions = [];
			_finallyActions = [];
			_isExecuting = new ReactiveProperty<bool>(false);
			_canExecuteObservable = Singletons.TrueReadOnlyReactiveProperty;

			ReentrancyHandler = IgnoreReentrantReentrancyHandler.Instance;

			if (canExecute != null)
			{
				CanExecuteObservable = canExecute.ToReadOnlyReactiveProperty(false);
			}
		}

		/// <summary>
		/// Creates a <see cref="AsyncCommand"/> and registers the given action.
		/// </summary>
		/// <param name="action">Action to execute when command is executed.</param>
		public AsyncCommand(Action action) : this()
		{
			Subscribe(action);
		}

		/// <summary>
		/// Creates a <see cref="AsyncCommand"/> and registers the given action.
		/// </summary>
		/// <param name="action">Action to execute when command is executed.</param>
		public AsyncCommand(Action<T?> action) : this()
		{
			Subscribe(action);
		}

		/// <summary>
		/// Creates a <see cref="AsyncCommand"/> and registers the given action.
		/// </summary>
		/// <param name="action">Action to execute when command is executed.</param>
		public AsyncCommand(Action<CancellationToken> action) : this()
		{
			Subscribe(action);
		}

		/// <summary>
		/// Creates a <see cref="AsyncCommand"/> and registers the given action.
		/// </summary>
		/// <param name="action">Action to execute when command is executed.</param>
		public AsyncCommand(Action<T?, CancellationToken> action) : this()
		{
			Subscribe(action);
		}

		/// <summary>
		/// Creates a <see cref="AsyncCommand"/> and registers the given action.
		/// </summary>
		/// <param name="action">Action to execute when command is executed.</param>
		public AsyncCommand(Func<Task> action) : this()
		{
			Subscribe(action);
		}

		/// <summary>
		/// Creates a <see cref="AsyncCommand"/> and registers the given action.
		/// </summary>
		/// <param name="action">Action to execute when command is executed.</param>
		public AsyncCommand(Func<T?, Task> action) : this()
		{
			Subscribe(action);
		}

		/// <summary>
		/// Creates a <see cref="AsyncCommand"/> and registers the given action.
		/// </summary>
		/// <param name="action">Action to execute when command is executed.</param>
		public AsyncCommand(Func<CancellationToken, Task> action) : this()
		{
			Subscribe(action);
		}

		/// <summary>
		/// Creates a <see cref="AsyncCommand"/> and registers the given action.
		/// </summary>
		/// <param name="action">Action to execute when command is executed.</param>
		public AsyncCommand(Func<T?, CancellationToken, Task> action) : this()
		{
			Subscribe(action);
		}

		/// <summary>
		/// Creates a <see cref="AsyncCommand"/> and registers the given action.
		/// </summary>
		/// <param name="canExecute">An observable that determines the can execute property. Can execute will always be true if this parameter is null.</param>
		/// <param name="action">Action to execute when command is executed.</param>
		public AsyncCommand(IObservable<bool>? canExecute, Action action) : this(canExecute)
		{
			Subscribe(action);
		}

		/// <summary>
		/// Creates a <see cref="AsyncCommand"/> and registers the given action.
		/// </summary>
		/// <param name="canExecute">An observable that determines the can execute property. Can execute will always be true if this parameter is null.</param>
		/// <param name="action">Action to execute when command is executed.</param>
		public AsyncCommand(IObservable<bool>? canExecute, Action<T?> action) : this(canExecute)
		{
			Subscribe(action);
		}

		/// <summary>
		/// Creates a <see cref="AsyncCommand"/> and registers the given action.
		/// </summary>
		/// <param name="canExecute">An observable that determines the can execute property. Can execute will always be true if this parameter is null.</param>
		/// <param name="action">Action to execute when command is executed.</param>
		public AsyncCommand(IObservable<bool>? canExecute, Action<CancellationToken> action) : this(canExecute)
		{
			Subscribe(action);
		}

		/// <summary>
		/// Creates a <see cref="AsyncCommand"/> and registers the given action.
		/// </summary>
		/// <param name="canExecute">An observable that determines the can execute property. Can execute will always be true if this parameter is null.</param>
		/// <param name="action">Action to execute when command is executed.</param>
		public AsyncCommand(IObservable<bool>? canExecute, Action<T?, CancellationToken> action) : this(canExecute)
		{
			Subscribe(action);
		}

		/// <summary>
		/// Creates a <see cref="AsyncCommand"/> and registers the given action.
		/// </summary>
		/// <param name="canExecute">An observable that determines the can execute property. Can execute will always be true if this parameter is null.</param>
		/// <param name="action">Action to execute when command is executed.</param>
		public AsyncCommand(IObservable<bool>? canExecute, Func<Task> action) : this(canExecute)
		{
			Subscribe(action);
		}

		/// <summary>
		/// Creates a <see cref="AsyncCommand"/> and registers the given action.
		/// </summary>
		/// <param name="canExecute">An observable that determines the can execute property. Can execute will always be true if this parameter is null.</param>
		/// <param name="action">Action to execute when command is executed.</param>
		public AsyncCommand(IObservable<bool>? canExecute, Func<T?, Task> action) : this(canExecute)
		{
			Subscribe(action);
		}

		/// <summary>
		/// Creates a <see cref="AsyncCommand"/> and registers the given action.
		/// </summary>
		/// <param name="canExecute">An observable that determines the can execute property. Can execute will always be true if this parameter is null.</param>
		/// <param name="action">Action to execute when command is executed.</param>
		public AsyncCommand(IObservable<bool>? canExecute, Func<CancellationToken, Task> action) : this(canExecute)
		{
			Subscribe(action);
		}

		/// <summary>
		/// Creates a <see cref="AsyncCommand"/> and registers the given action.
		/// </summary>
		/// <param name="canExecute">An observable that determines the can execute property. Can execute will always be true if this parameter is null.</param>
		/// <param name="action">Action to execute when command is executed.</param>
		public AsyncCommand(IObservable<bool>? canExecute, Func<T?, CancellationToken, Task> action) : this(canExecute)
		{
			Subscribe(action);
		}

		/// <inheritdoc />
		public IDisposable Subscribe(Action action, Order? order = null)
		{
			return Subscribe(
				(_, _) =>
				{
					action();
					return Task.CompletedTask;
				},
				order);
		}

		/// <inheritdoc />
		public IDisposable Subscribe(Action<T?> action, Order? order = null)
		{
			return Subscribe(
				(x, _) =>
				{
					action(x);
					return Task.CompletedTask;
				},
				order);
		}

		/// <inheritdoc />
		public IDisposable Subscribe(Action<CancellationToken> action, Order? order = null)
		{
			return Subscribe(
				(_, y) =>
				{
					action(y);
					return Task.CompletedTask;
				},
				order);
		}

		/// <inheritdoc />
		public IDisposable Subscribe(Action<T?, CancellationToken> action, Order? order = null)
		{
			return Subscribe(
				(x, y) =>
				{
					action(x, y);
					return Task.CompletedTask;
				},
				order);
		}

		/// <inheritdoc />
		public IDisposable Subscribe(Func<Task> action, Order? order = null)
		{
			return Subscribe(async (_, _) => await action(), order);
		}

		/// <inheritdoc />
		public IDisposable Subscribe(Func<T?, Task> action, Order? order = null)
		{
			return Subscribe(async (x, _) => await action(x), order);
		}

		/// <inheritdoc />
		public IDisposable Subscribe(Func<CancellationToken, Task> action, Order? order = null)
		{
			return Subscribe(async (_, y) => await action(y), order);
		}

		/// <inheritdoc />
		public IDisposable Subscribe(Func<T?, CancellationToken, Task> action, Order? order = null)
		{
			order ??= AsyncCommand.DefaultOrder;

			OrderedAction orderedAction = new(action, order);

			_actions.Add(orderedAction);
			_actions.Sort(ActionSorter.Instance);

			return new Subscription(this, orderedAction);
		}

		/// <inheritdoc />
		public IDisposable SubscribeFinally(Action action, Order? order = null)
		{
			return SubscribeFinally(
				(_, _) =>
				{
					action();
					return Task.CompletedTask;
				},
				order);
		}

		/// <inheritdoc />
		public IDisposable SubscribeFinally(Action<T?> action, Order? order = null)
		{
			return SubscribeFinally(
				(x, _) =>
				{
					action(x);
					return Task.CompletedTask;
				},
				order);
		}

		/// <inheritdoc />
		public IDisposable SubscribeFinally(Action<CancellationToken> action, Order? order = null)
		{
			return SubscribeFinally(
				(_, y) =>
				{
					action(y);
					return Task.CompletedTask;
				},
				order);
		}

		/// <inheritdoc />
		public IDisposable SubscribeFinally(Action<T?, CancellationToken> action, Order? order = null)
		{
			return SubscribeFinally(
				(x, y) =>
				{
					action(x, y);
					return Task.CompletedTask;
				},
				order);
		}

		/// <inheritdoc />
		public IDisposable SubscribeFinally(Func<Task> action, Order? order = null)
		{
			return SubscribeFinally(async (_, _) => await action(), order);
		}

		/// <inheritdoc />
		public IDisposable SubscribeFinally(Func<T?, Task> action, Order? order = null)
		{
			return SubscribeFinally(async (x, _) => await action(x), order);
		}

		/// <inheritdoc />
		public IDisposable SubscribeFinally(Func<CancellationToken, Task> action, Order? order = null)
		{
			return SubscribeFinally(async (_, y) => await action(y), order);
		}

		/// <inheritdoc />
		public IDisposable SubscribeFinally(Func<T?, CancellationToken, Task> action, Order? order = null)
		{
			order ??= AsyncCommand.DefaultOrder;

			OrderedAction orderedAction = new(action, order);

			_finallyActions.Add(orderedAction);
			_finallyActions.Sort(ActionSorter.Instance);

			return new Subscription(this, orderedAction);
		}

		/// <inheritdoc />
		bool System.Windows.Input.ICommand.CanExecute(object? parameter) => CanExecuteObservable.Value;

		/// <inheritdoc />
		async void System.Windows.Input.ICommand.Execute(object? parameter) => await Execute((T?)parameter);

		/// <summary>
		/// Executes the <see cref="AsyncCommand"/> with given parameter.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The actions will not be executed if <see cref="CanExecuteObservable"/>'s value is false.
		/// </para>
		/// <para>
		/// Actions will be executed according to the <see cref="Order"/> when they are registered.
		/// The action with the minimum order will be executed first. Actions with the same order will
		/// be executed concurrently.
		/// </para>
		/// </remarks>
		/// <param name="value">
		/// The parameter to pass to the actions. If this parameter implements the <see cref="ICancellable"/>
		/// interface, then its <see cref="ICancellable.IsCancelled"/> property will be checked before execution
		/// and no more actions will be executed if it is true.
		/// </param>
		/// <param name="cancellationToken">
		/// The cancellation token that will be passed to the actions and also get checked before execution
		/// of each action.
		/// </param>
		/// <returns>A task to be awaited.</returns>
		public async Task Execute(T? value, CancellationToken cancellationToken = default)
		{
			if (!CanExecuteObservable.Value)
			{
				return;
			}

			if (_isExecuting.Value && !ReentrancyHandler.AllowConcurrentExecution)
			{
				return;
			}

			TaskCompletionSource<bool> taskCompletionSource = new();
			PreExecuteResult preExecuteResult = default;

			try
			{
				preExecuteResult = ReentrancyHandler.PreExecute(cancellationToken);

				if (!preExecuteResult.ContinueExecution)
				{
					return;
				}

				_awaitableTask = taskCompletionSource;
				_isExecuting.Value = true;

				foreach (OrderedAction orderedAction in _actions)
				{
					if (cancellationToken.IsCancellationRequested)
					{
						break;
					}

					if (value is ICancellable { IsCancelled: true })
					{
						break;
					}

					await orderedAction.Action(value, cancellationToken);
				}
			}
			catch (Exception exception)
			{
				UnhandledExceptionEventArgs args = new(UnhandledExceptionSource.Action, exception);
				UnhandledException?.Invoke(this, args);

				if (args.Rethrow)
				{
					throw;
				}
			}
			finally
			{
				try
				{
					if (preExecuteResult.ContinueExecution)
					{
						foreach (OrderedAction orderedAction in _finallyActions)
						{
							await orderedAction.Action(value, cancellationToken);
						}
					}
				}
				catch (Exception exception)
				{
					UnhandledExceptionEventArgs args = new(UnhandledExceptionSource.FinallyAction, exception);
					UnhandledException?.Invoke(this, args);

					if (args.Rethrow)
					{
						throw;
					}
				}
				finally
				{
					ReentrancyHandler.PostExecute();

					_isExecuting.Value = false;
					taskCompletionSource.SetResult(true);
				}
			}
		}

		public TaskAwaiter GetAwaiter()
		{
			return ((Task?)_awaitableTask?.Task)?.GetAwaiter() ?? Task.CompletedTask.GetAwaiter();
		}
		
		private struct OrderedAction
		{
			public Func<T?, CancellationToken, Task> Action { get; }
			public Order Order { get; }

			public OrderedAction(Func<T?, CancellationToken, Task> action, Order order)
			{
				Action = action;
				Order = order;
			}
		}

		private class Subscription : IDisposable
		{
			private readonly AsyncCommand<T> _command;
			private readonly OrderedAction _action;

			public Subscription(AsyncCommand<T> command, OrderedAction action)
			{
				_command = command;
				_action = action;
			}

			public void Dispose()
			{
				_command._actions.Remove(_action);
			}
		}

		private class ActionSorter : IComparer<OrderedAction>
		{
			public static readonly ActionSorter Instance = new();

			public int Compare(OrderedAction x, OrderedAction y)
			{
				return Comparer<Order>.Default.Compare(x.Order, y.Order);
			}
		}

		void IObserver<bool>.OnCompleted()
		{
		}

		void IObserver<bool>.OnError(Exception error)
		{
		}

		void IObserver<bool>.OnNext(bool value)
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}