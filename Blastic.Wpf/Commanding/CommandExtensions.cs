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
			IViewAware context = null)
		{
			InputBinding inputBinding = new InputBinding(command, gesture);

			if (context == null)
			{
				Application.Current.MainWindow?.InputBindings.Add(inputBinding);
				return;
			}

			context.View.Subscribe(x =>
			{
				if (!(x is FrameworkElement frameworkElement))
				{
					return;
				}

				frameworkElement.InputBindings.Add(inputBinding);
			});
		}

		public static T WithInputGesture<T>(
			this T command,
			InputGesture gesture,
			IViewAware context = null)
			where T : ICommand
		{
			command.AddInputGesture(gesture, context);
			return command;
		}
	}
}