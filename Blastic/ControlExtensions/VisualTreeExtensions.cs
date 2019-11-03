using System.Windows;
using System.Windows.Media;

namespace Blastic.ControlExtensions
{
	public static class VisualTreeExtensions
	{
		public static T FindChild<T>(DependencyObject parent) where T : DependencyObject
		{
			if (parent == null)
			{
				return null;
			}

			T foundChild = null;

			int childrenCount = VisualTreeHelper.GetChildrenCount(parent);

			for (int i = 0; i < childrenCount; i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(parent, i);

				if (child is T result)
				{
					foundChild = result;
					break;
				}

				foundChild = FindChild<T>(child);

				if (foundChild != null)
				{
					break;
				}
			}

			return foundChild;
		}

		public static FrameworkElement FindChild(DependencyObject parent, string childName)
		{
			if (parent == null || string.IsNullOrWhiteSpace(childName))
			{
				return null;
			}

			FrameworkElement foundChild = null;

			int childrenCount = VisualTreeHelper.GetChildrenCount(parent);

			for (int i = 0; i < childrenCount; i++)
			{
				if (!(VisualTreeHelper.GetChild(parent, i) is FrameworkElement child))
				{
					continue;
				}

				if (child.Name == childName)
				{
					foundChild = child;
					break;
				}

				foundChild = FindChild(child, childName);

				if (foundChild == null)
				{
					continue;
				}

				if (foundChild.Name == childName)
				{
					break;
				}
			}

			return foundChild;
		}
	}
}