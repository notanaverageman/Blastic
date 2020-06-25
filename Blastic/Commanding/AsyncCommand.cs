using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Blastic.Ordering;
using Blastic.Platform;
using Blastic.Reactive;

namespace Blastic.Commanding
{
	public class AsyncCommand : AsyncCommand<object?>
	{
		public static readonly Order DefaultOrder = new Order();

		public AsyncCommand() : this((IObservable<bool>?) null)
		{
		}

		public AsyncCommand(IObservable<bool> canExecute, Func<AsyncCommandContext, Task> action) : this(canExecute)
		{
			Subscribe(action);
		}

		public AsyncCommand(IObservable<bool> canExecute, Func<Task> action) : this(canExecute)
		{
			Subscribe(action);
		}

		public AsyncCommand(Func<AsyncCommandContext, Task> action) : this()
		{
			Subscribe(action);
		}

		public AsyncCommand(Func<Task> action) : this()
		{
			Subscribe(action);
		}

		public AsyncCommand(IObservable<bool>? canExecute) : base(canExecute)
		{
		}

		public async Task Execute()
		{
			await Execute((object?)null);
		}

		public IDisposable Subscribe(Func<AsyncCommandContext, Task> action, Order? order = null)
		{
			return base.Subscribe(async x => await action(x), order);
		}
	}

	public class AsyncCommand<T> : ICommand
	{
		private readonly ConcurrentDictionary<Func<AsyncCommandContext<T>, Task>, Order> _actions;

		public event EventHandler? CanExecuteChanged;

		public IReadOnlyReactiveProperty<bool> CanExecuteObservable { get; }

		public AsyncCommand() : this((IObservable<bool>?) null)
		{
		}

		public AsyncCommand(IObservable<bool> canExecute, Func<AsyncCommandContext<T>, Task> action) : this(canExecute)
		{
			Subscribe(action);
		}

		public AsyncCommand(IObservable<bool> canExecute, Func<Task> action) : this(canExecute)
		{
			Subscribe(action);
		}

		public AsyncCommand(Func<AsyncCommandContext<T>, Task> action) : this()
		{
			Subscribe(action);
		}

		public AsyncCommand(Func<Task> action) : this()
		{
			Subscribe(action);
		}

		public AsyncCommand(IObservable<bool>? canExecute)
		{
			_actions = new ConcurrentDictionary<Func<AsyncCommandContext<T>, Task>, Order>();

			CanExecuteObservable = canExecute?.ToReadOnlyReactiveProperty() ?? Singletons.TrueReadOnlyReactiveProperty;

			CanExecuteObservable
				.ObserveOnUI()
				.Subscribe(_ => CanExecuteChanged?.Invoke(this, EventArgs.Empty));
		}

		public IDisposable Subscribe(Func<Task> action, Order? order = null)
		{
			return Subscribe(async x => await action(), order);
		}

		public IDisposable Subscribe(Func<AsyncCommandContext<T>, Task> action, Order? order = null)
		{
			order ??= AsyncCommand.DefaultOrder;
			_actions[action] = order;

			return new Subscription(this, action);
		}

		bool ICommand.CanExecute(object parameter) => CanExecute();
		async void ICommand.Execute(object parameter) => await Execute((T)parameter);

		public bool CanExecute()
		{
			return CanExecuteObservable.Value;
		}

		public async Task Execute(T parameter)
		{
			AsyncCommandContext<T> context = new AsyncCommandContext<T>(parameter);
			await Execute(context);
		}

		public async Task Execute(AsyncCommandContext<T> context)
		{
			if (!CanExecuteObservable.Value)
			{
				return;
			}

			if (context.ContinueExecution == false)
			{
				return;
			}

			IOrderedEnumerable<IGrouping<Order, Func<AsyncCommandContext<T>, Task>>> orderedActions = _actions.Keys
				.GroupBy(x => _actions[x])
				.OrderBy(x => x.Key);

			foreach (IGrouping<Order, Func<AsyncCommandContext<T>, Task>> actionGroup in orderedActions)
			{
				if (context.ContinueExecution == false)
				{
					break;
				}

				await Task.WhenAll(actionGroup.Select(x => x.Invoke(context)));
			}
		}

		private class Subscription : IDisposable
		{
			private readonly AsyncCommand<T> _command;
			private readonly Func<AsyncCommandContext<T>, Task> _action;

			public Subscription(AsyncCommand<T> command, Func<AsyncCommandContext<T>, Task> action)
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

	public static class AsyncCommandExtensions
	{
		public static AsyncCommand ToAsyncCommand(this IObservable<bool> canExecute)
		{
			return new AsyncCommand(canExecute);
		}

		public static AsyncCommand<T> ToAsyncCommand<T>(this IObservable<bool> canExecute)
		{
			return new AsyncCommand<T>(canExecute);
		}

		public static AsyncCommand WithSubscribe(
			this AsyncCommand command,
			Func<Task> action,
			Order? order = null)
		{
			command.Subscribe(action, order);
			return command;
		}

		public static AsyncCommand WithSubscribe(
			this AsyncCommand command,
			Func<AsyncCommandContext, Task> action,
			Order? order = null)
		{
			command.Subscribe(action, order);
			return command;
		}

		public static AsyncCommand<T> WithSubscribe<T>(
			this AsyncCommand<T> command,
			Func<AsyncCommandContext<T>, Task> action,
			Order? order = null)
		{
			command.Subscribe(action, order);
			return command;
		}
	}
}