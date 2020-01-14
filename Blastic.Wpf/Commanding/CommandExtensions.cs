using System;
using System.Windows;
using System.Windows.Input;
using Blastic.ViewManagement;

namespace Blastic.Wpf.Commanding
{
	public static class CommandExtensions
	{
		public static void AddInputGesture(
			this ICommand command,
			InputGesture gesture,
			IViewAware context)
		{
			context.View.Subscribe(x =>
			{
				if (!(x is FrameworkElement frameworkElement))
				{
					return;
				}

				frameworkElement.InputBindings.Add(new InputBinding(command, gesture));
			});
		}

		public static T WithInputGesture<T>(
			this T command,
			InputGesture gesture,
			IViewAware context)
			where T : ICommand
		{
			command.AddInputGesture(gesture, context);
			return command;
		}
	}
}