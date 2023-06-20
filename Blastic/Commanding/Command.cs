using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Commanding.Concurrency;
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
	public class Command<T> : ICommand, IReadOnlyCommand<T>, IObserver<bool>
	{
		private readonly IReactiveProperty<bool> _isExecuting;

		private ImmutableArray<OrderedAction> _actions;
		private ImmutableArray<OrderedAction> _finallyActions;

		private TaskCompletionSource<bool>? _awaitableTask;

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
		public IReentrancyHandler ReentrancyHandler { get; set; }

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
			_actions = ImmutableArray<OrderedAction>.Empty;
			_finallyActions = ImmutableArray<OrderedAction>.Empty;
			_isExecuting = new ReactiveProperty<bool>(false);

			ReentrancyHandler = IgnoreReentrantReentrancyHandler.Instance;
			CanExecuteObservable = canExecute?.ToReadOnlyReactiveProperty(false) ?? Singletons.TrueReadOnlyReactiveProperty;

			CanExecuteObservable
				.ObserveOnUI()
				.Subscribe(this);
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
			
			_actions = _actions.Add(orderedAction);
			_actions = _actions.Sort(ActionSorter.Instance);

			return new Subscription(this, orderedAction, isFinally: false);
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

			_finallyActions = _finallyActions.Add(orderedAction);
			_finallyActions = _finallyActions.Sort(ActionSorter.Instance);

			return new Subscription(this, orderedAction, isFinally: true);
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

					orderedAction.Action(value, cancellationToken);
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
							orderedAction.Action(value, cancellationToken);
						}
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
			private readonly bool _isFinally;

			public Subscription(Command<T> command, OrderedAction action, bool isFinally)
			{
				_command = command;
				_action = action;
				_isFinally = isFinally;
			}

			public void Dispose()
			{
				if (_isFinally)
				{
					_command._finallyActions = _command._finallyActions.Remove(_action);
				}
				else
				{
					_command._actions = _command._actions.Remove(_action);
				}
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