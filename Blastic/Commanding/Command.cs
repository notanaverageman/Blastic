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
	public class Command : Command<object?>
	{
		public static readonly Order DefaultOrder = new Order();

		public Command() : this((IObservable<bool>?) null)
		{
		}

		public Command(IObservable<bool>? canExecute) : base(canExecute)
		{
		}

		public Command(Action action) : this()
		{
			Subscribe(action);
		}

		public Command(Action<CancellationToken> action) : this()
		{
			Subscribe(action);
		}

		public Command(Func<Task> action) : this()
		{
			Subscribe(action);
		}

		public Command(Func<CancellationToken, Task> action) : this()
		{
			Subscribe(action);
		}

		public Command(IObservable<bool>? canExecute, Action action) : this(canExecute)
		{
			Subscribe(action);
		}

		public Command(IObservable<bool>? canExecute, Action<CancellationToken> action) : this(canExecute)
		{
			Subscribe(action);
		}

		public Command(IObservable<bool>? canExecute, Func<Task> action) : this(canExecute)
		{
			Subscribe(action);
		}

		public Command(IObservable<bool>? canExecute, Func<CancellationToken, Task> action) : this(canExecute)
		{
			Subscribe(action);
		}

		public async Task Execute()
		{
			await Execute(null);
		}
	}

	public class Command<T> : ICommand
	{
		private readonly ConcurrentDictionary<Func<T, CancellationToken, Task>, Order> _actions;

		public event EventHandler? CanExecuteChanged;

		public IReadOnlyReactiveProperty<bool> CanExecuteObservable { get; }

		public Command() : this((IObservable<bool>?) null)
		{
		}

		public Command(IObservable<bool>? canExecute)
		{
			_actions = new ConcurrentDictionary<Func<T, CancellationToken, Task>, Order>();

			CanExecuteObservable = canExecute?.ToReadOnlyReactiveProperty() ?? Singletons.TrueReadOnlyReactiveProperty;

			CanExecuteObservable
				.ObserveOnUI()
				.Subscribe(_ => CanExecuteChanged?.Invoke(this, EventArgs.Empty));
		}

		public Command(Action action) : this()
		{
			Subscribe(action);
		}

		public Command(Action<T> action) : this()
		{
			Subscribe(action);
		}

		public Command(Action<CancellationToken> action) : this()
		{
			Subscribe(action);
		}

		public Command(Action<T, CancellationToken> action) : this()
		{
			Subscribe(action);
		}

		public Command(Func<Task> action) : this()
		{
			Subscribe(action);
		}

		public Command(Func<T, Task> action) : this()
		{
			Subscribe(action);
		}

		public Command(Func<CancellationToken, Task> action) : this()
		{
			Subscribe(action);
		}

		public Command(Func<T, CancellationToken, Task> action) : this()
		{
			Subscribe(action);
		}

		public Command(IObservable<bool>? canExecute, Action action) : this(canExecute)
		{
			Subscribe(action);
		}

		public Command(IObservable<bool>? canExecute, Action<T> action) : this(canExecute)
		{
			Subscribe(action);
		}

		public Command(IObservable<bool>? canExecute, Action<CancellationToken> action) : this(canExecute)
		{
			Subscribe(action);
		}

		public Command(IObservable<bool>? canExecute, Action<T, CancellationToken> action) : this(canExecute)
		{
			Subscribe(action);
		}

		public Command(IObservable<bool>? canExecute, Func<Task> action) : this(canExecute)
		{
			Subscribe(action);
		}

		public Command(IObservable<bool>? canExecute, Func<T, Task> action) : this(canExecute)
		{
			Subscribe(action);
		}

		public Command(IObservable<bool>? canExecute, Func<CancellationToken, Task> action) : this(canExecute)
		{
			Subscribe(action);
		}

		public Command(IObservable<bool>? canExecute, Func<T, CancellationToken, Task> action) : this(canExecute)
		{
			Subscribe(action);
		}

		public IDisposable Subscribe(Action action, Order? order = null)
		{
			return Subscribe(
				(x, y) =>
				{
					action();
					return Task.CompletedTask;
				},
				order);
		}

		public IDisposable Subscribe(Action<T> action, Order? order = null)
		{
			return Subscribe(
				(x, y) =>
				{
					action(x);
					return Task.CompletedTask;
				},
				order);
		}

		public IDisposable Subscribe(Action<CancellationToken> action, Order? order = null)
		{
			return Subscribe(
				(x, y) =>
				{
					action(y);
					return Task.CompletedTask;
				},
				order);
		}

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

		public IDisposable Subscribe(Func<Task> action, Order? order = null)
		{
			return Subscribe(async (x, y) => await action(), order);
		}

		public IDisposable Subscribe(Func<T, Task> action, Order? order = null)
		{
			return Subscribe(async (x, y) => await action(x), order);
		}

		public IDisposable Subscribe(Func<CancellationToken, Task> action, Order? order = null)
		{
			return Subscribe(async (x, y) => await action(y), order);
		}

		public IDisposable Subscribe(Func<T, CancellationToken, Task> action, Order? order = null)
		{
			order ??= Command.DefaultOrder;
			_actions[action] = order;

			return new Subscription(this, action);
		}

		bool ICommand.CanExecute(object parameter) => CanExecute();
		async void ICommand.Execute(object parameter) => await Execute((T)parameter);

		public bool CanExecute()
		{
			return CanExecuteObservable.Value;
		}

		public async Task Execute(T value, CancellationToken cancellationToken = default)
		{
			if (!CanExecuteObservable.Value)
			{
				return;
			}

			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}

			IOrderedEnumerable<IGrouping<Order, Func<T, CancellationToken, Task>>> orderedActions = _actions.Keys
				.GroupBy(x => _actions[x])
				.OrderBy(x => x.Key);

			foreach (IGrouping<Order, Func<T, CancellationToken, Task>> actionGroup in orderedActions)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					break;
				}

				if (value is ICancellable cancellable && cancellable.IsCancelled)
				{
					break;
				}

				await Task.WhenAll(actionGroup.Select(x => x.Invoke(value, cancellationToken)));
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

	public static class CommandExtensions
	{
		public static Command ToCommand(this IObservable<bool> canExecute)
		{
			return new Command(canExecute);
		}

		public static Command<T> ToCommand<T>(this IObservable<bool> canExecute)
		{
			return new Command<T>(canExecute);
		}

		public static Command WithSubscribe(
			this Command command,
			Action action,
			Order? order = null)
		{
			command.Subscribe(action, order);
			return command;
		}

		public static Command WithSubscribe(
			this Command command,
			Action<CancellationToken> action,
			Order? order = null)
		{
			command.Subscribe(action, order);
			return command;
		}

		public static Command WithSubscribe(
			this Command command,
			Func<Task> action,
			Order? order = null)
		{
			command.Subscribe(action, order);
			return command;
		}

		public static Command WithSubscribe(
			this Command command,
			Func<CancellationToken, Task> action,
			Order? order = null)
		{
			command.Subscribe(action, order);
			return command;
		}

		public static Command<T> WithSubscribe<T>(
			this Command<T> command,
			Action action,
			Order? order = null)
		{
			command.Subscribe(action, order);
			return command;
		}

		public static Command<T> WithSubscribe<T>(
			this Command<T> command,
			Action<T> action,
			Order? order = null)
		{
			command.Subscribe(action, order);
			return command;
		}

		public static Command<T> WithSubscribe<T>(
			this Command<T> command,
			Action<CancellationToken> action,
			Order? order = null)
		{
			command.Subscribe(action, order);
			return command;
		}

		public static Command<T> WithSubscribe<T>(
			this Command<T> command,
			Action<T, CancellationToken> action,
			Order? order = null)
		{
			command.Subscribe(action, order);
			return command;
		}

		public static Command<T> WithSubscribe<T>(
			this Command<T> command,
			Func<Task> action,
			Order? order = null)
		{
			command.Subscribe(action, order);
			return command;
		}

		public static Command<T> WithSubscribe<T>(
			this Command<T> command,
			Func<T, Task> action,
			Order? order = null)
		{
			command.Subscribe(action, order);
			return command;
		}

		public static Command<T> WithSubscribe<T>(
			this Command<T> command,
			Func<CancellationToken, Task> action,
			Order? order = null)
		{
			command.Subscribe(action, order);
			return command;
		}

		public static Command<T> WithSubscribe<T>(
			this Command<T> command,
			Func<T, CancellationToken, Task> action,
			Order? order = null)
		{
			command.Subscribe(action, order);
			return command;
		}
	}
}