using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Blastic.ControlExtensions
{
    public class DataGridRowNumberBehavior
    {
		public static readonly DependencyProperty DisplayRowNumberProperty = DependencyProperty.RegisterAttached(
			nameof(DisplayRowNumberProperty).Replace("Property", ""),
			typeof(bool),
			typeof(DataGridRowNumberBehavior),
			new PropertyMetadata(default(bool), OnDisplayRowNumberChanged));
		public static bool GetDisplayRowNumber(DependencyObject obj) => (bool)obj.GetValue(DisplayRowNumberProperty);
		public static void SetDisplayRowNumber(DependencyObject obj, bool value) => obj.SetValue(DisplayRowNumberProperty, value);

		public static readonly DependencyProperty RowNumberOffsetProperty = DependencyProperty.RegisterAttached(
			nameof(RowNumberOffsetProperty).Replace("Property", ""),
			typeof(int),
			typeof(DataGridRowNumberBehavior),
			new PropertyMetadata(default(int)));
		public static int GetRowNumberOffset(DependencyObject obj) => (int)obj.GetValue(RowNumberOffsetProperty);
		public static void SetRowNumberOffset(DependencyObject obj, int value) => obj.SetValue(RowNumberOffsetProperty, value);

        private static void OnDisplayRowNumberChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
	        if (!(target is DataGrid dataGrid))
            {
                return;
            }

	        if (!(bool) e.NewValue)
	        {
		        return;
	        }

	        void LoadedRowHandler(object sender, DataGridRowEventArgs ea)
	        {
		        if (GetDisplayRowNumber(dataGrid) == false)
		        {
			        dataGrid.LoadingRow -= LoadedRowHandler;
			        return;
		        }

		        int rowNumberOffset = GetRowNumberOffset(dataGrid);

		        ea.Row.Header = ea.Row.GetIndex() + rowNumberOffset + 1;
	        }

	        dataGrid.LoadingRow += LoadedRowHandler;

	        void ItemsChangedHandler(object sender, ItemsChangedEventArgs ea)
	        {
		        if (GetDisplayRowNumber(dataGrid) == false)
		        {
			        dataGrid.ItemContainerGenerator.ItemsChanged -= ItemsChangedHandler;
			        return;
		        }
		        GetVisualChildCollection<DataGridRow>(dataGrid).ForEach(d => d.Header = d.GetIndex());
	        }

	        dataGrid.ItemContainerGenerator.ItemsChanged += ItemsChangedHandler;
        }

        private static List<T> GetVisualChildCollection<T>(object parent) where T : Visual
        {
            List<T> visualCollection = new List<T>();
            GetVisualChildCollection(parent as DependencyObject, visualCollection);
            return visualCollection;
        }

        private static void GetVisualChildCollection<T>(DependencyObject parent, List<T> visualCollection) where T : Visual
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                
	            if (child is T variable)
                {
                    visualCollection.Add(variable);
                }

	            GetVisualChildCollection(child, visualCollection);
            }
        }
    }
}