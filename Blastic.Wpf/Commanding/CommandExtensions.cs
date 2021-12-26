using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Blastic.ViewManagement;

namespace Blastic.Wpf.Commanding
{
	public static class CommandExtensions
	{
		public static void AddInputGesture(
			this ICommand command,
			InputGesture gesture,
			IViewAware? context = null)
		{
			InputBinding inputBinding = new(command, gesture);
			AddInputBinding(inputBinding, context);
		}

		public static void AddInputGesture<T>(
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

			AddInputBinding(inputBinding, context);
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

		private static void AddInputBinding(InputBinding inputBinding, IViewAware? context)
		{
			if (context == null)
			{
				Window? mainWindow = Application.Current.MainWindow;

				if (mainWindow == null)
				{
					throw new InvalidOperationException("Main window is not created.");
				}
				
				mainWindow.KeyDown += (_, args) =>
				{
					ModifierKeys modifierKeys = Keyboard.Modifiers;

					if (args.OriginalSource is TextBoxBase && modifierKeys is ModifierKeys.None or ModifierKeys.Shift)
					{
						return;
					}

					if (inputBinding.Gesture.Matches(mainWindow, args))
					{
						ICommand command = inputBinding.Command;
						object parameter = inputBinding.CommandParameter;

						if (command.CanExecute(parameter))
						{
							command.Execute(parameter);
						}

						args.Handled = true;
					}
				};
				
				return;
			}

			context.View.Subscribe(x =>
			{
				if (x is not FrameworkElement frameworkElement)
				{
					throw new InvalidOperationException("View does not inherit from FrameworkElement.");
				}

				frameworkElement.InputBindings.Add(inputBinding);
			});
		}
	}
}