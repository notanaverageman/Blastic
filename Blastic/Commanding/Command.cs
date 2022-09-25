using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
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
		public Command(IObservable<bool>? canExecute, Action action) : this(canExecute)
		{
			Subscribe(action);
		}

		/// <inheritdoc />
		public Command(IObservable<bool>? canExecute, Action<CancellationToken> action) : this(canExecute)
		{
			Subscribe(action);
		}
		
		/// <inheritdoc cref="Command{T}.Execute(T, CancellationToken)" />
		public void Execute(CancellationToken cancellationToken = default)
		{
			Execute(null, cancellationToken);
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
	/// SCommand's state can be observed via <see cref="CanExecuteObservable"/> and <see cref="IsExecuting"/>.
	/// Reentrancy can be disabled by setting <see cref="ReentrancyMode"/> method to
	/// <see cref="Commanding.ReentrancyMode.IgnoreReentrant"/> or <see cref="Commanding.ReentrancyMode.RunLatestCancelRunning"/>.
	/// </para>
	/// <para>
	/// You can register an action via the constructor or the <see cref="Subscribe(Action, Order?)"/> methods.
	/// <see cref="Subscribe(Action, Order?)"/> methods return an <see cref="IDisposable"/> that unregisters the
	/// action when disposed.
	/// </para>
	/// </remarks>
	/// <typeparam name="T"></typeparam>
	public class Command<T> : ICommand, IReadOnlyCommand<T>
	{
		private readonly List<OrderedAction> _actions;
		private readonly List<OrderedAction> _finallyActions;
		private readonly IReactiveProperty<bool> _isExecuting;
		private readonly SemaphoreSlim _semaphore;

		private CancellationTokenSource? _cancellationTokenSource;
		private TaskCompletionSource<bool>? _awaitableTask;
		private long _executionNumber;

		/// <inheritdoc />
		public event EventHandler? CanExecuteChanged;
		
		/// <inheritdoc />
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
			_actions = new List<OrderedAction>();
			_finallyActions = new List<OrderedAction>();
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
		public Command(Action<T?> action) : this()
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
		public Command(Action<T?, CancellationToken> action) : this()
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
		public Command(IObservable<bool>? canExecute, Action<T?> action) : this(canExecute)
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
		public Command(IObservable<bool>? canExecute, Action<T?, CancellationToken> action) : this(canExecute)
		{
			Subscribe(action);
		}
		
		/// <inheritdoc />
		public IDisposable Subscribe(Action action, Order? order = null)
		{
			return Subscribe((_, _) => action(), order);
		}

		/// <inheritdoc />
		public IDisposable Subscribe(Action<T?> action, Order? order = null)
		{
			return Subscribe((x, _) =>action(x), order);
		}

		/// <inheritdoc />
		public IDisposable Subscribe(Action<CancellationToken> action, Order? order = null)
		{
			return Subscribe((_, y) => action(y), order);
		}

		/// <inheritdoc />
		public IDisposable Subscribe(Action<T?, CancellationToken> action, Order? order = null)
		{
			order ??= Command.DefaultOrder;

			OrderedAction orderedAction = new(action, order);

			_actions.Add(orderedAction);
			_actions.Sort(ActionSorter.Instance);

			return new Subscription(this, orderedAction);
		}

		/// <inheritdoc />
		public IDisposable SubscribeFinally(Action action, Order? order = null)
		{
			return SubscribeFinally((_, _) => action(), order);
		}

		/// <inheritdoc />
		public IDisposable SubscribeFinally(Action<T?> action, Order? order = null)
		{
			return SubscribeFinally((x, _) => action(x), order);
		}

		/// <inheritdoc />
		public IDisposable SubscribeFinally(Action<CancellationToken> action, Order? order = null)
		{
			return SubscribeFinally((_, y) => action(y), order);
		}

		/// <inheritdoc />
		public IDisposable SubscribeFinally(Action<T?, CancellationToken> action, Order? order = null)
		{
			order ??= Command.DefaultOrder;

			OrderedAction orderedAction = new(action, order);

			_finallyActions.Add(orderedAction);
			_finallyActions.Sort(ActionSorter.Instance);

			return new Subscription(this, orderedAction);
		}
		
		/// <inheritdoc />
		bool System.Windows.Input.ICommand.CanExecute(object? parameter) => CanExecuteObservable.Value;

		/// <inheritdoc />
		void System.Windows.Input.ICommand.Execute(object? parameter) => Execute((T?)parameter);

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
		public void Execute(T? value, CancellationToken cancellationToken = default)
		{
			if (!CanExecuteObservable.Value)
			{
				return;
			}

			if (_isExecuting.Value && ReentrancyMode == ReentrancyMode.IgnoreReentrant)
			{
				return;
			}

			bool acquiredSemaphore = false;
			TaskCompletionSource<bool> taskCompletionSource = new();

			try
			{
				CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
				cancellationToken = cancellationTokenSource.Token;

				if (ReentrancyMode is ReentrancyMode.RunLatest or ReentrancyMode.RunLatestCancelRunning)
				{
					long currentExecutionNumber = Interlocked.Increment(ref _executionNumber);

					try
					{
						if (ReentrancyMode is ReentrancyMode.RunLatestCancelRunning)
						{
							_cancellationTokenSource?.Cancel();
						}

						_semaphore.Wait(cancellationToken);
						acquiredSemaphore = true;
					}
					catch (OperationCanceledException)
					{
						return;
					}
					
					if (currentExecutionNumber < _executionNumber)
					{
						if (ReentrancyMode is ReentrancyMode.RunLatestCancelRunning)
						{
							cancellationTokenSource.Cancel();
						}

						return;
					}
				}

				if (ReentrancyMode is ReentrancyMode.RunLatestCancelRunning)
				{
					_cancellationTokenSource = cancellationTokenSource;
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

					orderedAction.Action(value, cancellationToken);
				}
			}
			finally
			{
				try
				{
					foreach (OrderedAction orderedAction in _finallyActions)
					{
						orderedAction.Action(value, cancellationToken);
					}
				}
				finally
				{
					if (acquiredSemaphore)
					{
						_semaphore.Release();
					}

					_isExecuting.Value = false;
					taskCompletionSource.SetResult(true);

					_cancellationTokenSource = null;
				}
			}
		}

		public TaskAwaiter GetAwaiter()
		{
			return ((Task?)_awaitableTask?.Task)?.GetAwaiter() ?? Task.CompletedTask.GetAwaiter();
		}

		private struct OrderedAction
		{
			public Action<T?, CancellationToken> Action { get; }
			public Order Order { get; }

			public OrderedAction(Action<T?, CancellationToken> action, Order order)
			{
				Action = action;
				Order = order;
			}
		}

		private class Subscription : IDisposable
		{
			private readonly Command<T> _command;
			private readonly OrderedAction _action;

			public Subscription(Command<T> command, OrderedAction action)
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
	}
}