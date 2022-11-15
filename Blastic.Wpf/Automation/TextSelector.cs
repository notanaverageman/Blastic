using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Blastic.Reactive;
using Blastic.ViewManagement;

namespace Blastic.Wpf.Automation
{
	public static partial class AutomationExtensions
	{
		public static Task SetSelection(
			this IViewAware viewAware,
			IReactiveProperty<string> property,
			int start,
			int length)
		{
			FrameworkElement? element = viewAware.GetView(property);

			if (element is not TextBox textBox)
			{
				return Task.CompletedTask;
			}

			textBox.Focus();

			textBox.SelectionStart = start;
			textBox.SelectionLength = length;

			return Task.CompletedTask;
		}
	}
}