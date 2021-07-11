using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Blastic.Ordering;
using Blastic.Platform;
using Blastic.Reactive;

namespace Blastic.Commanding
{
	/// <inheritdoc cref="Command{T}"/>
	public class Command : Command<object?>
	{
		/// <summary>
		/// The default order of the actions if their order is not specified.
		/// </summary>
		public static readonly Order DefaultOrder = new();

		/// <inheritdoc />
		public Command() : this((IObservable<bool>?)null)
		{
		}

		/// <inheritdoc />
		public Command(IObservable<bool>? canExecute) : base(canExecute)
		{
		}

		/// <inheritdoc />
		public Command(Action action) : this()
		{
			Subscribe(action);
		}

		/// <inheritdoc />
		public Command(Action<CancellationToken> action) : this()
		{
			Subscribe(action);
		}

		/// <inheritdoc />
		public Command(Func<Task> action) : this()
		{
			Subscribe(action);
		}

		/// <inheritdoc />
		public Command(Func<CancellationToken, Task> action) : this()
		{
			Subscribe(action);
		}

		/// <inheritdoc />
		public Command(IObservable<bool>? canExecute, Action action) : this(canExecute)
		{
			Subscribe(action);
		}

		/// <inheritdoc />
		public Command(IObservable<bool>? canExecute, Action<CancellationToken> action) : this(canExecute)
		{
			Subscribe(action);
		}

		/// <inheritdoc />
		public Command(IObservable<bool>? canExecute, Func<Task> action) : this(canExecute)
		{
			Subscribe(action);
		}

		/// <inheritdoc />
		public Command(IObservable<bool>? canExecute, Func<CancellationToken, Task> action) : this(canExecute)
		{
			Subscribe(action);
		}

		/// <inheritdoc cref="Command{T}.Execute(T, CancellationToken)" />
		public async Task Execute()
		{
			await Execute(null);
		}
	}

	/// <summary>
	/// An implementation of <see cref="ICommand"/> with reactive patterns.
	/// </summary>
	/// <remarks>
	/// <para>
	/// A <see cref="Command"/> can hold a number of actions that can be ordered with
	/// <see cref="Order"/> classes. When the command is executed, these actions will
	/// be executed based on their order. Actions with lesser order will be executed
	/// first and actions with the same order will be executed concurrently.
	/// </para>
	/// <para>
	/// Command's state can be observed via <see cref="CanExecuteObservable"/> and <see cref="IsExecuting"/>.
	/// Reentrancy can be disabled by setting <see cref="ReentrancyMode"/> method to
	/// <see cref="Commanding.ReentrancyMode.IgnoreReentrant"/> or <see cref="Commanding.ReentrancyMode.CancelRunning"/>.
	/// </para>
	/// <para>
	/// You can register an action via the constructor or the <see cref="Subscribe(Action, Order?)"/> methods.
	/// <see cref="Subscribe(Action, Order?)"/> methods return an <see cref="IDisposable"/> that unregisters the
	/// action when disposed.
	/// </para>
	/// </remarks>
	/// <typeparam name="T"></typeparam>
	public class Command<T> : ICommand
	{
		private readonly ConcurrentDictionary<Func<T, CancellationToken, Task>, Order> _actions;
		private readonly IReactiveProperty<bool> _isExecuting;
		private readonly SemaphoreSlim _semaphore;

		private long _executionNumber;

		/// <inheritdoc />
		public event EventHandler? CanExecuteChanged;

		/// <summary>
		/// An observable property that emits when command's CanExecute property changes.
		/// </summary>
		public IReadOnlyReactiveProperty<bool> CanExecuteObservable { get; }

		/// <summary>
		/// An observable property that emits true when the <see cref="Command"/> starts execution and
		/// emits false when the <see cref="Command"/> finishes the execution.
		/// </summary>
		public IReadOnlyReactiveProperty<bool> IsExecuting => _isExecuting;

		/// <summary>
		/// Property that defines the behavior of the command when it is executed concurrently.
		/// </summary>
		public ReentrancyMode ReentrancyMode { get; set; }

		/// <summary>
		/// Default constructor that creates an always executable <see cref="Command"/>.
		/// </summary>
		public Command() : this((IObservable<bool>?)null)
		{
		}

		/// <summary>
		/// Creates a <see cref="Command"/> with given observable for can execute property.
		/// </summary>
		/// <param name="canExecute">An observable that determines the can execute property. Can execute will always be true if this parameter is null.</param>
		public Command(IObservable<bool>? canExecute)
		{
			_actions = new ConcurrentDictionary<Func<T, CancellationToken, Task>, Order>();
			_isExecuting = new ReactiveProperty<bool>();
			_semaphore = new SemaphoreSlim(1, 1);

			CanExecuteObservable = canExecute?.ToReadOnlyReactiveProperty() ?? Singletons.TrueReadOnlyReactiveProperty;

			CanExecuteObservable
				.ObserveOnUI()
				.Subscribe(_ => CanExecuteChanged?.Invoke(this, EventArgs.Empty));
		}

		/// <summary>
		/// Creates a <see cref="Command"/> and registers the given action.
		/// </summary>
		/// <param name="action">Action to execute when command is executed.</param>
		public Command(Action action) : this()
		{
			Subscribe(action);
		}

		/// <summary>
		/// Creates a <see cref="Command"/> and registers the given action.
		/// </summary>
		/// <param name="action">Action to execute when command is executed.</param>
		public Command(Action<T> action) : this()
		{
			Subscribe(action);
		}

		/// <summary>
		/// Creates a <see cref="Command"/> and registers the given action.
		/// </summary>
		/// <param name="action">Action to execute when command is executed.</param>
		public Command(Action<CancellationToken> action) : this()
		{
			Subscribe(action);
		}

		/// <summary>
		/// Creates a <see cref="Command"/> and registers the given action.
		/// </summary>
		/// <param name="action">Action to execute when command is executed.</param>
		public Command(Action<T, CancellationToken> action) : this()
		{
			Subscribe(action);
		}

		/// <summary>
		/// Creates a <see cref="Command"/> and registers the given action.
		/// </summary>
		/// <param name="action">Action to execute when command is executed.</param>
		public Command(Func<Task> action) : this()
		{
			Subscribe(action);
		}

		/// <summary>
		/// Creates a <see cref="Command"/> and registers the given action.
		/// </summary>
		/// <param name="action">Action to execute when command is executed.</param>
		public Command(Func<T, Task> action) : this()
		{
			Subscribe(action);
		}

		/// <summary>
		/// Creates a <see cref="Command"/> and registers the given action.
		/// </summary>
		/// <param name="action">Action to execute when command is executed.</param>
		public Command(Func<CancellationToken, Task> action) : this()
		{
			Subscribe(action);
		}

		/// <summary>
		/// Creates a <see cref="Command"/> and registers the given action.
		/// </summary>
		/// <param name="action">Action to execute when command is executed.</param>
		public Command(Func<T, CancellationToken, Task> action) : this()
		{
			Subscribe(action);
		}

		/// <summary>
		/// Creates a <see cref="Command"/> and registers the given action.
		/// </summary>
		/// <param name="canExecute">An observable that determines the can execute property. Can execute will always be true if this parameter is null.</param>
		/// <param name="action">Action to execute when command is executed.</param>
		public Command(IObservable<bool>? canExecute, Action action) : this(canExecute)
		{
			Subscribe(action);
		}

		/// <summary>
		/// Creates a <see cref="Command"/> and registers the given action.
		/// </summary>
		/// <param name="canExecute">An observable that determines the can execute property. Can execute will always be true if this parameter is null.</param>
		/// <param name="action">Action to execute when command is executed.</param>
		public Command(IObservable<bool>? canExecute, Action<T> action) : this(canExecute)
		{
			Subscribe(action);
		}

		/// <summary>
		/// Creates a <see cref="Command"/> and registers the given action.
		/// </summary>
		/// <param name="canExecute">An observable that determines the can execute property. Can execute will always be true if this parameter is null.</param>
		/// <param name="action">Action to execute when command is executed.</param>
		public Command(IObservable<bool>? canExecute, Action<CancellationToken> action) : this(canExecute)
		{
			Subscribe(action);
		}

		/// <summary>
		/// Creates a <see cref="Command"/> and registers the given action.
		/// </summary>
		/// <param name="canExecute">An observable that determines the can execute property. Can execute will always be true if this parameter is null.</param>
		/// <param name="action">Action to execute when command is executed.</param>
		public Command(IObservable<bool>? canExecute, Action<T, CancellationToken> action) : this(canExecute)
		{
			Subscribe(action);
		}

		/// <summary>
		/// Creates a <see cref="Command"/> and registers the given action.
		/// </summary>
		/// <param name="canExecute">An observable that determines the can execute property. Can execute will always be true if this parameter is null.</param>
		/// <param name="action">Action to execute when command is executed.</param>
		public Command(IObservable<bool>? canExecute, Func<Task> action) : this(canExecute)
		{
			Subscribe(action);
		}

		/// <summary>
		/// Creates a <see cref="Command"/> and registers the given action.
		/// </summary>
		/// <param name="canExecute">An observable that determines the can execute property. Can execute will always be true if this parameter is null.</param>
		/// <param name="action">Action to execute when command is executed.</param>
		public Command(IObservable<bool>? canExecute, Func<T, Task> action) : this(canExecute)
		{
			Subscribe(action);
		}

		/// <summary>
		/// Creates a <see cref="Command"/> and registers the given action.
		/// </summary>
		/// <param name="canExecute">An observable that determines the can execute property. Can execute will always be true if this parameter is null.</param>
		/// <param name="action">Action to execute when command is executed.</param>
		public Command(IObservable<bool>? canExecute, Func<CancellationToken, Task> action) : this(canExecute)
		{
			Subscribe(action);
		}

		/// <summary>
		/// Creates a <see cref="Command"/> and registers the given action.
		/// </summary>
		/// <param name="canExecute">An observable that determines the can execute property. Can execute will always be true if this parameter is null.</param>
		/// <param name="action">Action to execute when command is executed.</param>
		public Command(IObservable<bool>? canExecute, Func<T, CancellationToken, Task> action) : this(canExecute)
		{
			Subscribe(action);
		}

		/// <summary>
		/// Registers the given action to be executed when the <see cref="Command"/> is executed.
		/// </summary>
		/// <param name="action">The action to execute.</param>
		/// <param name="order">Order of the action among other actions.</param>
		/// <returns>An <see cref="IDisposable"/> that unregisters the action when disposed.</returns>
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

		/// <summary>
		/// Registers the given action to be executed when the <see cref="Command"/> is executed.
		/// </summary>
		/// <param name="action">The action to execute.</param>
		/// <param name="order">Order of the action among other actions.</param>
		/// <returns>An <see cref="IDisposable"/> that unregisters the action when disposed.</returns>
		public IDisposable Subscribe(Action<T> action, Order? order = null)
		{
			return Subscribe(
				(x, _) =>
				{
					action(x);
					return Task.CompletedTask;
				},
				order);
		}

		/// <summary>
		/// Registers the given action to be executed when the <see cref="Command"/> is executed.
		/// </summary>
		/// <param name="action">The action to execute.</param>
		/// <param name="order">Order of the action among other actions.</param>
		/// <returns>An <see cref="IDisposable"/> that unregisters the action when disposed.</returns>
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

		/// <summary>
		/// Registers the given action to be executed when the <see cref="Command"/> is executed.
		/// </summary>
		/// <param name="action">The action to execute.</param>
		/// <param name="order">Order of the action among other actions.</param>
		/// <returns>An <see cref="IDisposable"/> that unregisters the action when disposed.</returns>
		public IDisposable Subscribe(Action<T, CancellationToken> action, Order? order = null)
		{
			return Subscribe(
				(x, y) =>
				{
					action(x, y);
					return Task.CompletedTask;
				},
				order);
		}

		/// <summary>
		/// Registers the given action to be executed when the <see cref="Command"/> is executed.
		/// </summary>
		/// <param name="action">The action to execute.</param>
		/// <param name="order">Order of the action among other actions.</param>
		/// <returns>An <see cref="IDisposable"/> that unregisters the action when disposed.</returns>
		public IDisposable Subscribe(Func<Task> action, Order? order = null)
		{
			return Subscribe(async (_, _) => await action(), order);
		}

		/// <summary>
		/// Registers the given action to be executed when the <see cref="Command"/> is executed.
		/// </summary>
		/// <param name="action">The action to execute.</param>
		/// <param name="order">Order of the action among other actions.</param>
		/// <returns>An <see cref="IDisposable"/> that unregisters the action when disposed.</returns>
		public IDisposable Subscribe(Func<T, Task> action, Order? order = null)
		{
			return Subscribe(async (x, _) => await action(x), order);
		}

		/// <summary>
		/// Registers the given action to be executed when the <see cref="Command"/> is executed.
		/// </summary>
		/// <param name="action">The action to execute.</param>
		/// <param name="order">Order of the action among other actions.</param>
		/// <returns>An <see cref="IDisposable"/> that unregisters the action when disposed.</returns>
		public IDisposable Subscribe(Func<CancellationToken, Task> action, Order? order = null)
		{
			return Subscribe(async (_, y) => await action(y), order);
		}

		/// <summary>
		/// Registers the given action to be executed when the <see cref="Command"/> is executed.
		/// </summary>
		/// <param name="action">The action to execute.</param>
		/// <param name="order">Order of the action among other actions.</param>
		/// <returns>An <see cref="IDisposable"/> that unregisters the action when disposed.</returns>
		public IDisposable Subscribe(Func<T, CancellationToken, Task> action, Order? order = null)
		{
			order ??= Command.DefaultOrder;
			_actions[action] = order;

			return new Subscription(this, action);
		}

		/// <inheritdoc />
		bool ICommand.CanExecute(object parameter) => CanExecuteObservable.Value;

		/// <inheritdoc />
		async void ICommand.Execute(object parameter) => await Execute((T)parameter);

		/// <summary>
		/// Executes the <see cref="Command"/> with given parameter.
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
		public async Task Execute(T value, CancellationToken cancellationToken = default)
		{
			if (!CanExecuteObservable.Value)
			{
				return;
			}

			if (_isExecuting.Value && ReentrancyMode == ReentrancyMode.IgnoreReentrant)
			{
				return;
			}

			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}

			try
			{
				if (ReentrancyMode == ReentrancyMode.CancelRunning)
				{
					long currentExecutionNumber = Interlocked.Increment(ref _executionNumber);

					try
					{
						await _semaphore.WaitAsync(cancellationToken);
					}
					catch (OperationCanceledException)
					{
						return;
					}
					
					if (currentExecutionNumber < _executionNumber)
					{
						return;
					}
				}

				_isExecuting.Value = true;

				IOrderedEnumerable<IGrouping<Order, Func<T, CancellationToken, Task>>> orderedActions = _actions.Keys
					.GroupBy(x => _actions[x])
					.OrderBy(x => x.Key);

				foreach (IGrouping<Order, Func<T, CancellationToken, Task>> actionGroup in orderedActions)
				{
					if (cancellationToken.IsCancellationRequested)
					{
						break;
					}

					if (value is ICancellable { IsCancelled: true })
					{
						break;
					}

					await Task.WhenAll(actionGroup.Select(x => x.Invoke(value, cancellationToken)));
				}
			}
			finally
			{
				if (ReentrancyMode == ReentrancyMode.CancelRunning)
				{
					_semaphore.Release();
				}

				_isExecuting.Value = false;
			}
		}

		private class Subscription : IDisposable
		{
			private readonly Command<T> _command;
			private readonly Func<T, CancellationToken, Task> _action;

			public Subscription(Command<T> command, Func<T, CancellationToken, Task> action)
			{
				_command = command;
				_action = action;
			}

			public void Dispose()
			{
				_command._actions.TryRemove(_action, out _);
			}
		}
	}
}