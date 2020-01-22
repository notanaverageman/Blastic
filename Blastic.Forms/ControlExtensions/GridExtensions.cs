using Xamarin.Forms;

namespace Blastic.Forms.ControlExtensions
{
	public class GridExtensions
	{
		public static readonly BindableProperty RowDefinitionsProperty = BindableProperty.CreateAttached(
			nameof(RowDefinitionsProperty).Replace("Property", ""),
			typeof(string),
			typeof(GridExtensions),
			default(string),
			propertyChanged: OnRowDefinitionsChanged);
		public static string GetRowDefinitions(BindableObject obj) => (string)obj.GetValue(RowDefinitionsProperty);
		public static void SetRowDefinitions(BindableObject obj, string value) => obj.SetValue(RowDefinitionsProperty, value);

		public static readonly BindableProperty ColumnDefinitionsProperty = BindableProperty.CreateAttached(
			nameof(ColumnDefinitionsProperty).Replace("Property", ""),
			typeof(string),
			typeof(GridExtensions),
			default(string),
			propertyChanged: OnColumnDefinitionsChanged);
		public static string GetColumnDefinitions(BindableObject obj) => (string)obj.GetValue(ColumnDefinitionsProperty);
		public static void SetColumnDefinitions(BindableObject obj, string value) => obj.SetValue(ColumnDefinitionsProperty, value);

		private static void OnRowDefinitionsChanged(BindableObject bindable, object oldValue, object newValue)
		{
			if (!(bindable is Grid targetGrid) || !(newValue is string rows))
			{
				return;
			}

			targetGrid.RowDefinitions.Clear();
			string[] rowDefinitions = rows.Split(',');

			foreach (string rowDefinition in rowDefinitions)
			{
				if (rowDefinition.Trim() == "")
				{
					targetGrid.RowDefinitions.Add(new RowDefinition());
				}
				else
				{
					targetGrid.RowDefinitions.Add(new RowDefinition
					{
						Height = ParseLength(rowDefinition)
					});
				}
			}
		}

		private static void OnColumnDefinitionsChanged(BindableObject bindable, object oldValue, object newValue)
		{
			if (!(bindable is Grid targetGrid) || !(newValue is string columns))
			{
				return;
			}

			targetGrid.ColumnDefinitions.Clear();
			string[] columnDefinitions = columns.Split(',');

			foreach (string columnDefinition in columnDefinitions)
			{
				if (columnDefinition.Trim() == "")
				{
					targetGrid.ColumnDefinitions.Add(new ColumnDefinition());
				}
				else
				{
					targetGrid.ColumnDefinitions.Add(new ColumnDefinition
					{
						Width = ParseLength(columnDefinition)
					});
				}
			}
		}

		private static GridLength ParseLength(string length)
		{
			length = length.Trim();

			if (length.ToLowerInvariant().Equals("auto"))
			{
				return new GridLength(0, GridUnitType.Auto);
			}
			if (length.Contains("*"))
			{
				length = length.Replace("*", "");

				if (string.IsNullOrEmpty(length))
				{
					length = "1";
				}

				return new GridLength(double.Parse(length), GridUnitType.Star);
			}

			return new GridLength(double.Parse(length), GridUnitType.Absolute);
		}
	}
}