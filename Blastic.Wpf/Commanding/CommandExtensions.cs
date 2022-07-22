using System;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Blastic.Reactive;
using Blastic.ViewManagement;

namespace Blastic.Wpf.Commanding
{
	public static class CommandExtensions
	{
		public static IDisposable AddInputGesture(
			this ICommand command,
			InputGesture gesture,
			IViewAware? context = null)
		{
			InputBinding inputBinding = new(command, gesture);
			return AddInputBinding(inputBinding, context);
		}

		public static IDisposable AddInputGesture<T>(
			this T command,
			Key key,
			ModifierKeys modifierKeys = ModifierKeys.None,
			IViewAware? context = null)
			where T : ICommand
		{
			InputBinding inputBinding = new KeyBinding
			{
				Command = command,
				Key = key,
				Modifiers = modifierKeys
			};

			return AddInputBinding(inputBinding, context);
		}

		public static T WithInputGesture<T>(
			this T command,
			InputGesture gesture,
			IViewAware? context = null)
			where T : ICommand
		{
			command.AddInputGesture(gesture, context);
			return command;
		}

		public static T WithInputGesture<T>(
			this T command,
			Key key,
			ModifierKeys modifierKeys = ModifierKeys.None,
			IViewAware? context = null)
			where T : ICommand
		{
			AddInputGesture(command, key, modifierKeys, context);
			return command;
		}

		private static IDisposable AddInputBinding(InputBinding inputBinding, IViewAware? context)
		{
			if (context != null)
			{
				context.View
					.WithPrevious()
					.Subscribe(x =>
					{
						object? previousView = x.Item1;
						object? currentView = x.Item2;

						if (previousView is FrameworkElement previousElement)
						{
							previousElement.InputBindings.Remove(inputBinding);
						}

						if (currentView is FrameworkElement frameworkElement)
						{
							frameworkElement.InputBindings.Add(inputBinding);
						}
					});

				return Disposable.Create(() =>
				{
					if (context.View.Value is FrameworkElement frameworkElement)
					{
						frameworkElement.InputBindings.Remove(inputBinding);
					}
				});
			}

			Window? mainWindow = Application.Current.MainWindow;

			if (mainWindow == null)
			{
				throw new InvalidOperationException("Main window is not created.");
			}

			void OnKeyDown(object _, KeyEventArgs keyEventArgs)
			{
				ModifierKeys modifierKeys = Keyboard.Modifiers;

				if (keyEventArgs.OriginalSource is TextBoxBase && modifierKeys is ModifierKeys.None or ModifierKeys.Shift)
				{
					return;
				}

				if (inputBinding.Gesture.Matches(mainWindow, keyEventArgs))
				{
					ICommand command = inputBinding.Command;
					object parameter = inputBinding.CommandParameter;

					if (command.CanExecute(parameter))
					{
						command.Execute(parameter);
					}

					keyEventArgs.Handled = true;
				}
			}

			mainWindow.KeyDown += OnKeyDown;

			return Disposable.Create(() =>
			{
				mainWindow.KeyDown -= OnKeyDown;
			});
		}
	}
}