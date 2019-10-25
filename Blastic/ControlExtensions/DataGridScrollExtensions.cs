using System.Windows;
using System.Windows.Controls;

namespace Blastic.ControlExtensions
{
	public static class DataGridScrollExtensions
	{
		public static readonly DependencyProperty TrackSelectedItemProperty = DependencyProperty.RegisterAttached(
			nameof(TrackSelectedItemProperty).Replace("Property", ""),
			typeof(bool),
			typeof(DataGridScrollExtensions),
			new PropertyMetadata(default, OnPropertyChanged));
		public static bool GetTrackSelectedItem(DependencyObject obj) => (bool)obj.GetValue(TrackSelectedItemProperty);
		public static void SetTrackSelectedItem(DependencyObject obj, bool value) => obj.SetValue(TrackSelectedItemProperty, value);

		public static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!(d is DataGrid dataGrid))
			{
				return;
			}

			bool newValue = (bool)e.NewValue;

			if (newValue)
			{
				dataGrid.SelectionChanged += SelectionChanged;
			}
		}

		private static void SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			DataGrid dataGrid = (DataGrid)sender;

			if (dataGrid.SelectedItem == null)
			{
				return;
			}

			dataGrid.UpdateLayout();
			dataGrid.ScrollIntoView(dataGrid.SelectedItem);
		}
	}
}