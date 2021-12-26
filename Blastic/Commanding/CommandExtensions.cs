using System;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Ordering;

namespace Blastic.Commanding
{
	public static class CommandExtensions
	{
		/// <summary>
		/// Create a <see cref="Command"/> with given can execute observable.
		/// </summary>
		/// <param name="canExecute">The can execute observable.</param>
		/// <returns>A command with given can execute observable.</returns>
		public static Command ToCommand(this IObservable<bool> canExecute)
		{
			return new Command(canExecute);
		}

		/// <summary>
		/// Create a <see cref="Command"/> with given can execute observable.
		/// </summary>
		/// <param name="canExecute">The can execute observable.</param>
		/// <returns>A command with given can execute observable.</returns>
		public static Command<T> ToCommand<T>(this IObservable<bool> canExecute)
		{
			return new Command<T>(canExecute);
		}

		/// <summary>
		/// Create a <see cref="AsyncCommand"/> with given can execute observable.
		/// </summary>
		/// <param name="canExecute">The can execute observable.</param>
		/// <returns>A command with given can execute observable.</returns>
		public static AsyncCommand ToAsyncCommand(this IObservable<bool> canExecute)
		{
			return new AsyncCommand(canExecute);
		}

		/// <summary>
		/// Create a <see cref="AsyncCommand"/> with given can execute observable.
		/// </summary>
		/// <param name="canExecute">The can execute observable.</param>
		/// <returns>A command with given can execute observable.</returns>
		public static AsyncCommand<T> ToAsyncCommand<T>(this IObservable<bool> canExecute)
		{
			return new AsyncCommand<T>(canExecute);
		}

		/// <summary>
		/// Fluent method that sets the reentrancy mode and returns the command.
		/// </summary>
		/// <param name="command">The command whose property to be set.</param>
		/// <param name="reentrancyMode">Reentrancy mode to set.</param>
		/// <returns>The given command.</returns>
		public static Command WithReentrancyMode(this Command command, ReentrancyMode reentrancyMode)
		{
			command.ReentrancyMode = reentrancyMode;
			return command;
		}

		/// <summary>
		/// Fluent method that sets the reentrancy mode and returns the command.
		/// </summary>
		/// <param name="command">The command whose property to be set.</param>
		/// <param name="reentrancyMode">Reentrancy mode to set.</param>
		/// <returns>The given command.</returns>
		public static Command<T> WithReentrancyMode<T>(this Command<T> command, ReentrancyMode reentrancyMode)
		{
			command.ReentrancyMode = reentrancyMode;
			return command;
		}

		/// <summary>
		/// Fluent method that sets the reentrancy mode and returns the command.
		/// </summary>
		/// <param name="command">The command whose property to be set.</param>
		/// <param name="reentrancyMode">Reentrancy mode to set.</param>
		/// <returns>The given command.</returns>
		public static AsyncCommand WithReentrancyMode(this AsyncCommand command, ReentrancyMode reentrancyMode)
		{
			command.ReentrancyMode = reentrancyMode;
			return command;
		}

		/// <summary>
		/// Fluent method that sets the reentrancy mode and returns the command.
		/// </summary>
		/// <param name="command">The command whose property to be set.</param>
		/// <param name="reentrancyMode">Reentrancy mode to set.</param>
		/// <returns>The given command.</returns>
		public static AsyncCommand<T> WithReentrancyMode<T>(this AsyncCommand<T> command, ReentrancyMode reentrancyMode)
		{
			command.ReentrancyMode = reentrancyMode;
			return command;
		}

		/// <summary>
		/// Fluent method that registers the given action and returns the command.
		/// </summary>
		/// <param name="command">The command to be subscribed.</param>
		/// <param name="action">The action to register.</param>
		/// <param name="order">Order of the action.</param>
		/// <returns>The given command.</returns>
		public static Command WithSubscribe(
			this Command command,
			Action action,
			Order? order = null)
		{
			command.Subscribe(action, order);
			return command;
		}

		/// <summary>
		/// Fluent method that registers the given action and returns the command.
		/// </summary>
		/// <param name="command">The command to be subscribed.</param>
		/// <param name="action">The action to register.</param>
		/// <param name="order">Order of the action.</param>
		/// <returns>The given command.</returns>
		public static Command WithSubscribe(
			this Command command,
			Action<CancellationToken> action,
			Order? order = null)
		{
			command.Subscribe(action, order);
			return command;
		}
		
		/// <summary>
		/// Fluent method that registers the given action and returns the command.
		/// </summary>
		/// <param name="command">The command to be subscribed.</param>
		/// <param name="action">The action to register.</param>
		/// <param name="order">Order of the action.</param>
		/// <returns>The given command.</returns>
		public static AsyncCommand WithSubscribe(
			this AsyncCommand command,
			Action action,
			Order? order = null)
		{
			command.Subscribe(action, order);
			return command;
		}

		/// <summary>
		/// Fluent method that registers the given action and returns the command.
		/// </summary>
		/// <param name="command">The command to be subscribed.</param>
		/// <param name="action">The action to register.</param>
		/// <param name="order">Order of the action.</param>
		/// <returns>The given command.</returns>
		public static AsyncCommand WithSubscribe(
			this AsyncCommand command,
			Action<CancellationToken> action,
			Order? order = null)
		{
			command.Subscribe(action, order);
			return command;
		}

		/// <summary>
		/// Fluent method that registers the given action and returns the command.
		/// </summary>
		/// <param name="command">The command to be subscribed.</param>
		/// <param name="action">The action to register.</param>
		/// <param name="order">Order of the action.</param>
		/// <returns>The given command.</returns>
		public static AsyncCommand WithSubscribe(
			this AsyncCommand command,
			Func<Task> action,
			Order? order = null)
		{
			command.Subscribe(action, order);
			return command;
		}

		/// <summary>
		/// Fluent method that registers the given action and returns the command.
		/// </summary>
		/// <param name="command">The command to be subscribed.</param>
		/// <param name="action">The action to register.</param>
		/// <param name="order">Order of the action.</param>
		/// <returns>The given command.</returns>
		public static AsyncCommand WithSubscribe(
			this AsyncCommand command,
			Func<CancellationToken, Task> action,
			Order? order = null)
		{
			command.Subscribe(action, order);
			return command;
		}

		/// <summary>
		/// Fluent method that registers the given action and returns the command.
		/// </summary>
		/// <param name="command">The command to be subscribed.</param>
		/// <param name="action">The action to register.</param>
		/// <param name="order">Order of the action.</param>
		/// <returns>The given command.</returns>
		public static Command WithSubscribeFinally(
			this Command command,
			Action action,
			Order? order = null)
		{
			command.SubscribeFinally(action, order);
			return command;
		}

		/// <summary>
		/// Fluent method that registers the given action and returns the command.
		/// </summary>
		/// <param name="command">The command to be subscribed.</param>
		/// <param name="action">The action to register.</param>
		/// <param name="order">Order of the action.</param>
		/// <returns>The given command.</returns>
		public static Command WithSubscribeFinally(
			this Command command,
			Action<CancellationToken> action,
			Order? order = null)
		{
			command.SubscribeFinally(action, order);
			return command;
		}
		
		/// <summary>
		/// Fluent method that registers the given action and returns the command.
		/// </summary>
		/// <param name="command">The command to be subscribed.</param>
		/// <param name="action">The action to register.</param>
		/// <param name="order">Order of the action.</param>
		/// <returns>The given command.</returns>
		public static AsyncCommand WithSubscribeFinally(
			this AsyncCommand command,
			Action action,
			Order? order = null)
		{
			command.SubscribeFinally(action, order);
			return command;
		}

		/// <summary>
		/// Fluent method that registers the given action and returns the command.
		/// </summary>
		/// <param name="command">The command to be subscribed.</param>
		/// <param name="action">The action to register.</param>
		/// <param name="order">Order of the action.</param>
		/// <returns>The given command.</returns>
		public static AsyncCommand WithSubscribeFinally(
			this AsyncCommand command,
			Action<CancellationToken> action,
			Order? order = null)
		{
			command.SubscribeFinally(action, order);
			return command;
		}

		/// <summary>
		/// Fluent method that registers the given action and returns the command.
		/// </summary>
		/// <param name="command">The command to be subscribed.</param>
		/// <param name="action">The action to register.</param>
		/// <param name="order">Order of the action.</param>
		/// <returns>The given command.</returns>
		public static AsyncCommand WithSubscribeFinally(
			this AsyncCommand command,
			Func<Task> action,
			Order? order = null)
		{
			command.SubscribeFinally(action, order);
			return command;
		}

		/// <summary>
		/// Fluent method that registers the given action and returns the command.
		/// </summary>
		/// <param name="command">The command to be subscribed.</param>
		/// <param name="action">The action to register.</param>
		/// <param name="order">Order of the action.</param>
		/// <returns>The given command.</returns>
		public static AsyncCommand WithSubscribeFinally(
			this AsyncCommand command,
			Func<CancellationToken, Task> action,
			Order? order = null)
		{
			command.SubscribeFinally(action, order);
			return command;
		}

		/// <summary>
		/// Fluent method that registers the given action and returns the command.
		/// </summary>
		/// <param name="command">The command to be subscribed.</param>
		/// <param name="action">The action to register.</param>
		/// <param name="order">Order of the action.</param>
		/// <returns>The given command.</returns>
		public static Command<T> WithSubscribe<T>(
			this Command<T> command,
			Action action,
			Order? order = null)
		{
			command.Subscribe(action, order);
			return command;
		}

		/// <summary>
		/// Fluent method that registers the given action and returns the command.
		/// </summary>
		/// <param name="command">The command to be subscribed.</param>
		/// <param name="action">The action to register.</param>
		/// <param name="order">Order of the action.</param>
		/// <returns>The given command.</returns>
		public static Command<T> WithSubscribe<T>(
			this Command<T> command,
			Action<T?> action,
			Order? order = null)
		{
			command.Subscribe(action, order);
			return command;
		}

		/// <summary>
		/// Fluent method that registers the given action and returns the command.
		/// </summary>
		/// <param name="command">The command to be subscribed.</param>
		/// <param name="action">The action to register.</param>
		/// <param name="order">Order of the action.</param>
		/// <returns>The given command.</returns>
		public static Command<T> WithSubscribe<T>(
			this Command<T> command,
			Action<CancellationToken> action,
			Order? order = null)
		{
			command.Subscribe(action, order);
			return command;
		}

		/// <summary>
		/// Fluent method that registers the given action and returns the command.
		/// </summary>
		/// <param name="command">The command to be subscribed.</param>
		/// <param name="action">The action to register.</param>
		/// <param name="order">Order of the action.</param>
		/// <returns>The given command.</returns>
		public static Command<T> WithSubscribe<T>(
			this Command<T> command,
			Action<T?, CancellationToken> action,
			Order? order = null)
		{
			command.Subscribe(action, order);
			return command;
		}

		/// <summary>
		/// Fluent method that registers the given action and returns the command.
		/// </summary>
		/// <param name="command">The command to be subscribed.</param>
		/// <param name="action">The action to register.</param>
		/// <param name="order">Order of the action.</param>
		/// <returns>The given command.</returns>
		public static AsyncCommand<T> WithSubscribe<T>(
			this AsyncCommand<T> command,
			Action action,
			Order? order = null)
		{
			command.Subscribe(action, order);
			return command;
		}

		/// <summary>
		/// Fluent method that registers the given action and returns the command.
		/// </summary>
		/// <param name="command">The command to be subscribed.</param>
		/// <param name="action">The action to register.</param>
		/// <param name="order">Order of the action.</param>
		/// <returns>The given command.</returns>
		public static AsyncCommand<T> WithSubscribe<T>(
			this AsyncCommand<T> command,
			Action<T?> action,
			Order? order = null)
		{
			command.Subscribe(action, order);
			return command;
		}

		/// <summary>
		/// Fluent method that registers the given action and returns the command.
		/// </summary>
		/// <param name="command">The command to be subscribed.</param>
		/// <param name="action">The action to register.</param>
		/// <param name="order">Order of the action.</param>
		/// <returns>The given command.</returns>
		public static AsyncCommand<T> WithSubscribe<T>(
			this AsyncCommand<T> command,
			Action<CancellationToken> action,
			Order? order = null)
		{
			command.Subscribe(action, order);
			return command;
		}

		/// <summary>
		/// Fluent method that registers the given action and returns the command.
		/// </summary>
		/// <param name="command">The command to be subscribed.</param>
		/// <param name="action">The action to register.</param>
		/// <param name="order">Order of the action.</param>
		/// <returns>The given command.</returns>
		public static AsyncCommand<T> WithSubscribe<T>(
			this AsyncCommand<T> command,
			Action<T?, CancellationToken> action,
			Order? order = null)
		{
			command.Subscribe(action, order);
			return command;
		}

		/// <summary>
		/// Fluent method that registers the given action and returns the command.
		/// </summary>
		/// <param name="command">The command to be subscribed.</param>
		/// <param name="action">The action to register.</param>
		/// <param name="order">Order of the action.</param>
		/// <returns>The given command.</returns>
		public static AsyncCommand<T> WithSubscribe<T>(
			this AsyncCommand<T> command,
			Func<Task> action,
			Order? order = null)
		{
			command.Subscribe(action, order);
			return command;
		}

		/// <summary>
		/// Fluent method that registers the given action and returns the command.
		/// </summary>
		/// <param name="command">The command to be subscribed.</param>
		/// <param name="action">The action to register.</param>
		/// <param name="order">Order of the action.</param>
		/// <returns>The given command.</returns>
		public static AsyncCommand<T> WithSubscribe<T>(
			this AsyncCommand<T> command,
			Func<T?, Task> action,
			Order? order = null)
		{
			command.Subscribe(action, order);
			return command;
		}

		/// <summary>
		/// Fluent method that registers the given action and returns the command.
		/// </summary>
		/// <param name="command">The command to be subscribed.</param>
		/// <param name="action">The action to register.</param>
		/// <param name="order">Order of the action.</param>
		/// <returns>The given command.</returns>
		public static AsyncCommand<T> WithSubscribe<T>(
			this AsyncCommand<T> command,
			Func<CancellationToken, Task> action,
			Order? order = null)
		{
			command.Subscribe(action, order);
			return command;
		}

		/// <summary>
		/// Fluent method that registers the given action and returns the command.
		/// </summary>
		/// <param name="command">The command to be subscribed.</param>
		/// <param name="action">The action to register.</param>
		/// <param name="order">Order of the action.</param>
		/// <returns>The given command.</returns>
		public static AsyncCommand<T> WithSubscribe<T>(
			this AsyncCommand<T> command,
			Func<T?, CancellationToken, Task> action,
			Order? order = null)
		{
			command.Subscribe(action, order);
			return command;
		}

		/// <summary>
		/// Fluent method that registers the given action and returns the command.
		/// </summary>
		/// <param name="command">The command to be subscribed.</param>
		/// <param name="action">The action to register.</param>
		/// <param name="order">Order of the action.</param>
		/// <returns>The given command.</returns>
		public static Command<T> WithSubscribeFinally<T>(
			this Command<T> command,
			Action action,
			Order? order = null)
		{
			command.SubscribeFinally(action, order);
			return command;
		}

		/// <summary>
		/// Fluent method that registers the given action and returns the command.
		/// </summary>
		/// <param name="command">The command to be subscribed.</param>
		/// <param name="action">The action to register.</param>
		/// <param name="order">Order of the action.</param>
		/// <returns>The given command.</returns>
		public static Command<T> WithSubscribeFinally<T>(
			this Command<T> command,
			Action<T?> action,
			Order? order = null)
		{
			command.SubscribeFinally(action, order);
			return command;
		}

		/// <summary>
		/// Fluent method that registers the given action and returns the command.
		/// </summary>
		/// <param name="command">The command to be subscribed.</param>
		/// <param name="action">The action to register.</param>
		/// <param name="order">Order of the action.</param>
		/// <returns>The given command.</returns>
		public static Command<T> WithSubscribeFinally<T>(
			this Command<T> command,
			Action<CancellationToken> action,
			Order? order = null)
		{
			command.SubscribeFinally(action, order);
			return command;
		}

		/// <summary>
		/// Fluent method that registers the given action and returns the command.
		/// </summary>
		/// <param name="command">The command to be subscribed.</param>
		/// <param name="action">The action to register.</param>
		/// <param name="order">Order of the action.</param>
		/// <returns>The given command.</returns>
		public static Command<T> WithSubscribeFinally<T>(
			this Command<T> command,
			Action<T?, CancellationToken> action,
			Order? order = null)
		{
			command.SubscribeFinally(action, order);
			return command;
		}

		/// <summary>
		/// Fluent method that registers the given action and returns the command.
		/// </summary>
		/// <param name="command">The command to be subscribed.</param>
		/// <param name="action">The action to register.</param>
		/// <param name="order">Order of the action.</param>
		/// <returns>The given command.</returns>
		public static AsyncCommand<T> WithSubscribeFinally<T>(
			this AsyncCommand<T> command,
			Action action,
			Order? order = null)
		{
			command.SubscribeFinally(action, order);
			return command;
		}

		/// <summary>
		/// Fluent method that registers the given action and returns the command.
		/// </summary>
		/// <param name="command">The command to be subscribed.</param>
		/// <param name="action">The action to register.</param>
		/// <param name="order">Order of the action.</param>
		/// <returns>The given command.</returns>
		public static AsyncCommand<T> WithSubscribeFinally<T>(
			this AsyncCommand<T> command,
			Action<T?> action,
			Order? order = null)
		{
			command.SubscribeFinally(action, order);
			return command;
		}

		/// <summary>
		/// Fluent method that registers the given action and returns the command.
		/// </summary>
		/// <param name="command">The command to be subscribed.</param>
		/// <param name="action">The action to register.</param>
		/// <param name="order">Order of the action.</param>
		/// <returns>The given command.</returns>
		public static AsyncCommand<T> WithSubscribeFinally<T>(
			this AsyncCommand<T> command,
			Action<CancellationToken> action,
			Order? order = null)
		{
			command.SubscribeFinally(action, order);
			return command;
		}

		/// <summary>
		/// Fluent method that registers the given action and returns the command.
		/// </summary>
		/// <param name="command">The command to be subscribed.</param>
		/// <param name="action">The action to register.</param>
		/// <param name="order">Order of the action.</param>
		/// <returns>The given command.</returns>
		public static AsyncCommand<T> WithSubscribeFinally<T>(
			this AsyncCommand<T> command,
			Action<T?, CancellationToken> action,
			Order? order = null)
		{
			command.SubscribeFinally(action, order);
			return command;
		}

		/// <summary>
		/// Fluent method that registers the given action and returns the command.
		/// </summary>
		/// <param name="command">The command to be subscribed.</param>
		/// <param name="action">The action to register.</param>
		/// <param name="order">Order of the action.</param>
		/// <returns>The given command.</returns>
		public static AsyncCommand<T> WithSubscribeFinally<T>(
			this AsyncCommand<T> command,
			Func<Task> action,
			Order? order = null)
		{
			command.SubscribeFinally(action, order);
			return command;
		}

		/// <summary>
		/// Fluent method that registers the given action and returns the command.
		/// </summary>
		/// <param name="command">The command to be subscribed.</param>
		/// <param name="action">The action to register.</param>
		/// <param name="order">Order of the action.</param>
		/// <returns>The given command.</returns>
		public static AsyncCommand<T> WithSubscribeFinally<T>(
			this AsyncCommand<T> command,
			Func<T?, Task> action,
			Order? order = null)
		{
			command.SubscribeFinally(action, order);
			return command;
		}

		/// <summary>
		/// Fluent method that registers the given action and returns the command.
		/// </summary>
		/// <param name="command">The command to be subscribed.</param>
		/// <param name="action">The action to register.</param>
		/// <param name="order">Order of the action.</param>
		/// <returns>The given command.</returns>
		public static AsyncCommand<T> WithSubscribeFinally<T>(
			this AsyncCommand<T> command,
			Func<CancellationToken, Task> action,
			Order? order = null)
		{
			command.SubscribeFinally(action, order);
			return command;
		}

		/// <summary>
		/// Fluent method that registers the given action and returns the command.
		/// </summary>
		/// <param name="command">The command to be subscribed.</param>
		/// <param name="action">The action to register.</param>
		/// <param name="order">Order of the action.</param>
		/// <returns>The given command.</returns>
		public static AsyncCommand<T> WithSubscribeFinally<T>(
			this AsyncCommand<T> command,
			Func<T?, CancellationToken, Task> action,
			Order? order = null)
		{
			command.SubscribeFinally(action, order);
			return command;
		}
	}
}