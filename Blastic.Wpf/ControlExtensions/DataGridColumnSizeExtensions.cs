using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Blastic.ControlExtensions
{
	public class DataGridColumnSizeExtensions
	{
		public static readonly DependencyProperty FixColumnFillSizeProperty = DependencyProperty.RegisterAttached(
			nameof(FixColumnFillSizeProperty).Replace("Property", ""),
			typeof(bool),
			typeof(DataGridColumnSizeExtensions),
			new PropertyMetadata(default(bool), OnFixColumnFillSizeChanged));
		public static bool GetFixColumnFillSizeProperty(DependencyObject obj) => (bool) obj.GetValue(FixColumnFillSizeProperty);
		public static void SetFixColumnFillSizeProperty(DependencyObject obj, bool value) => obj.SetValue(FixColumnFillSizeProperty, value);

		public static readonly DependencyProperty DataGridItemsSourceProperty = DependencyProperty.RegisterAttached(
			nameof(DataGridItemsSourceProperty).Replace("Property", ""),
			typeof(INotifyCollectionChanged),
			typeof(DataGridColumnSizeExtensions),
			new PropertyMetadata(default(INotifyCollectionChanged)));
		public static INotifyCollectionChanged GetDataGridItemsSource(DependencyObject obj) => (INotifyCollectionChanged)obj.GetValue(DataGridItemsSourceProperty);
		public static void SetDataGridItemsSource(DependencyObject obj, INotifyCollectionChanged value) => obj.SetValue(DataGridItemsSourceProperty, value);

		public static readonly DependencyProperty CollectionChangedActionProperty = DependencyProperty.RegisterAttached(
			nameof(CollectionChangedActionProperty).Replace("Property", ""),
			typeof(NotifyCollectionChangedEventHandler),
			typeof(DataGridColumnSizeExtensions),
			new PropertyMetadata(default(NotifyCollectionChangedEventHandler)));
		public static NotifyCollectionChangedEventHandler GetCollectionChangedAction(DependencyObject obj) => (NotifyCollectionChangedEventHandler)obj.GetValue(CollectionChangedActionProperty);
		public static void SetCollectionChangedAction(DependencyObject obj, NotifyCollectionChangedEventHandler value) => obj.SetValue(CollectionChangedActionProperty, value);

		private static void OnFixColumnFillSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DataGrid dataGrid = d as DataGrid;

			if (dataGrid == null)
			{
				return;
			}

			DependencyPropertyDescriptor descriptor = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, typeof(DataGrid));

			if ((e.NewValue as bool?) == true)
			{
				descriptor?.AddValueChanged(dataGrid, OnItemsSourceChanged);
			}
			else
			{
				descriptor?.RemoveValueChanged(dataGrid, OnItemsSourceChanged);
			}
		}

		private static void OnItemsSourceChanged(object sender, EventArgs e)
		{
			DataGrid dataGrid = sender as DataGrid;

			if (dataGrid == null)
			{
				return;
			}

			INotifyCollectionChanged oldItemsSource = GetDataGridItemsSource(dataGrid);

			if (oldItemsSource != null)
			{
				NotifyCollectionChangedEventHandler collectionChangedAction = GetCollectionChangedAction(dataGrid);
				oldItemsSource.CollectionChanged -= collectionChangedAction;
			}

			INotifyCollectionChanged observableCollection = dataGrid.ItemsSource as INotifyCollectionChanged;

			if (observableCollection == null)
			{
				return;
			}

			void Action(object o, NotifyCollectionChangedEventArgs a)
			{
				FixColumnSizes(dataGrid);
			}

			SetCollectionChangedAction(dataGrid, Action);
			observableCollection.CollectionChanged += Action;

			FixColumnSizes(dataGrid);
			SetDataGridItemsSource(dataGrid, (INotifyCollectionChanged)dataGrid.ItemsSource);
		}

		private static void FixColumnSizes(DataGrid dataGrid)
		{
			DataGridColumn firstColumn = dataGrid?.Columns.FirstOrDefault();

			if (firstColumn == null)
			{
				return;
			}

			firstColumn.Width = 0;
			dataGrid.UpdateLayout();
			firstColumn.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
		}
	}
}