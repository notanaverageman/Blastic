using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Windows.Input;
using Blastic.Ordering;
using Blastic.Platform;
using Blastic.Reactive;

namespace Blastic.Commanding
{
	public class Command : Command<object>
	{
		public static readonly Order DefaultOrder = new Order();

		public Command() : this((IObservable<bool>)null)
		{
		}

		public Command(IObservable<bool> canExecute, Action<CommandContext> action) : this(canExecute)
		{
			Subscribe(action);
		}

		public Command(IObservable<bool> canExecute, Action action) : this(canExecute)
		{
			Subscribe(action);
		}

		public Command(Action<CommandContext> action) : this()
		{
			Subscribe(action);
		}

		public Command(Action action) : this()
		{
			Subscribe(action);
		}

		public Command(IObservable<bool> canExecute) : base(canExecute)
		{
		}

		public void Execute()
		{
			Execute(null);
		}

		public IDisposable Subscribe(Action<CommandContext> action, Order order = null)
		{
			return base.Subscribe(action, order);
		}
	}

	public class Command<T> : ICommand
	{
		private readonly ConcurrentDictionary<Action<CommandContext<T>>, Order> _actions;

		public event EventHandler CanExecuteChanged;

		public IReadOnlyReactiveProperty<bool> CanExecuteObservable { get; }

		public Command() : this((IObservable<bool>) null)
		{
		}

		public Command(IObservable<bool> canExecute, Action<CommandContext<T>> action) : this(canExecute)
		{
			Subscribe(action);
		}

		public Command(IObservable<bool> canExecute, Action action) : this(canExecute)
		{
			Subscribe(action);
		}

		public Command(Action<CommandContext<T>> action) : this()
		{
			Subscribe(action);
		}

		public Command(Action action) : this()
		{
			Subscribe(action);
		}

		public Command(IObservable<bool> canExecute)
		{
			_actions = new ConcurrentDictionary<Action<CommandContext<T>>, Order>();

			CanExecuteObservable = canExecute?.ToReadOnlyReactiveProperty();
			CanExecuteObservable ??= new ReactiveProperty<bool>(true);

			CanExecuteObservable
				.ObserveOnUI()
				.Subscribe(_ => CanExecuteChanged?.Invoke(this, EventArgs.Empty));
		}

		public IDisposable Subscribe(Action action, Order order = null)
		{
			return Subscribe(x => action(), order);
		}

		public IDisposable Subscribe(Action<CommandContext<T>> action, Order order = null)
		{
			order ??= AsyncCommand.DefaultOrder;
			_actions[action] = order;

			return new Subscription(this, action);
		}

		bool ICommand.CanExecute(object parameter) => CanExecute();
		void ICommand.Execute(object parameter) => Execute((T)parameter);

		public bool CanExecute()
		{
			return CanExecuteObservable.Value;
		}

		public void Execute(T parameter)
		{
			CommandContext<T> context = new CommandContext<T>(parameter);
			Execute(context);
		}

		public void Execute(CommandContext<T> context)
		{
			if (!CanExecuteObservable.Value)
			{
				return;
			}

			if (!context.ContinueExecution)
			{
				return;
			}

			IOrderedEnumerable<IGrouping<Order, Action<CommandContext<T>>>> orderedActions = _actions.Keys
				.GroupBy(x => _actions[x])
				.OrderBy(x => x.Key);

			foreach (IGrouping<Order, Action<CommandContext<T>>> actionGroup in orderedActions)
			{
				if (!context.ContinueExecution)
				{
					break;
				}

				foreach (Action<CommandContext<T>> action in actionGroup)
				{
					action(context);
				}
			}
		}

		private class Subscription : IDisposable
		{
			private readonly Command<T> _command;
			private readonly Action<CommandContext<T>> _action;

			public Subscription(Command<T> command, Action<CommandContext<T>> action)
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
			Order order = null)
		{
			command.Subscribe(action, order);
			return command;
		}

		public static Command WithSubscribe(
			this Command command,
			Action<CommandContext> action,
			Order order = null)
		{
			command.Subscribe(action, order);
			return command;
		}

		public static Command<T> WithSubscribe<T>(
			this Command<T> command,
			Action<CommandContext<T>> action,
			Order order = null)
		{
			command.Subscribe(action, order);
			return command;
		}
	}
}