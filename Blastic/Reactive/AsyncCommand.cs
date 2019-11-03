using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Blastic.Common;
using Reactive.Bindings;

namespace Blastic.Reactive
{
	public class AsyncCommand : AsyncCommand<object>
	{
		public async Task Execute()
		{
			await Execute(null);
		}

		public IDisposable Subscribe(Func<AsyncCommandContext, Task> action, Order order = null)
		{
			return base.Subscribe(async x => await action(x), order);
		}
	}

	public class AsyncCommand<T> : ICommand, IDisposable
	{
		public static readonly Order DefaultOrder = new Order();

		private readonly ConcurrentDictionary<Func<AsyncCommandContext<T>, Task>, Order> _actions;
		private readonly IReadOnlyReactiveProperty<bool> _canExecute;

		public event EventHandler CanExecuteChanged;

		public AsyncCommand() : this(null)
		{
		}

		public AsyncCommand(IObservable<bool> canExecute)
		{
			_actions = new ConcurrentDictionary<Func<AsyncCommandContext<T>, Task>, Order>();

			_canExecute = canExecute?.ToReadOnlyReactiveProperty();
			_canExecute ??= new ReactiveProperty<bool>(true);

			_canExecute.Subscribe(_ => CanExecuteChanged?.Invoke(this, EventArgs.Empty));
		}

		public IDisposable Subscribe(Func<AsyncCommandContext<T>, Task> action, Order order = null)
		{
			order ??= DefaultOrder;
			_actions[action] = order;

			return new Subscription(this, action);
		}

		bool ICommand.CanExecute(object parameter) => CanExecute();
		async void ICommand.Execute(object parameter) => await Execute((T)parameter);

		public bool CanExecute()
		{
			return _canExecute.Value;
		}

		public async Task Execute(T parameter)
		{
			AsyncCommandContext<T> context = new AsyncCommandContext<T>(parameter);
			await Execute(context);
		}

		public async Task Execute(AsyncCommandContext<T> context)
		{
			if (!_canExecute.Value)
			{
				return;
			}

			if (!context.ContinueExecution)
			{
				return;
			}

			IOrderedEnumerable<IGrouping<Order, Func<AsyncCommandContext<T>, Task>>> orderedActions = _actions.Keys
				.GroupBy(x => _actions[x])
				.OrderBy(x => x.Key);

			foreach (IGrouping<Order, Func<AsyncCommandContext<T>, Task>> actionGroup in orderedActions)
			{
				if (!context.ContinueExecution)
				{
					break;
				}

				await Task.WhenAll(actionGroup.Select(x => x.Invoke(context)));
			}
		}

		public void Dispose()
		{
			_canExecute.Dispose();
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
		public static AsyncCommand<T> ToAsyncCommand<T>(this IObservable<bool> canExecute)
		{
			return new AsyncCommand<T>(canExecute);
		}

		public static AsyncCommand WithSubscribe(
			this AsyncCommand command,
			Func<AsyncCommandContext, Task> action,
			Order order = null)
		{
			command.Subscribe(action, order);
			return command;
		}

		public static AsyncCommand<T> WithSubscribe<T>(
			this AsyncCommand<T> command,
			Func<AsyncCommandContext<T>, Task> action,
			Order order = null)
		{
			command.Subscribe(action, order);
			return command;
		}
	}
}