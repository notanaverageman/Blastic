using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Blastic.Controls.DynamicControls;
using Blastic.Controls.DynamicControls.Elements;

namespace Blastic.Uno.Shared.Controls.DynamicControls
{
	public sealed partial class DynamicControl
	{
		public static readonly DependencyProperty ModelProperty = DependencyProperty.Register(
			nameof(ModelProperty).Replace("Property", ""),
			typeof(DynamicModel),
			typeof(DynamicControl),
			new PropertyMetadata(default(DynamicModel), OnFormChanged));
		public DynamicModel Model
		{
			get => (DynamicModel)GetValue(ModelProperty);
			set => SetValue(ModelProperty, value);
		}

		public DynamicControl()
		{
			InitializeComponent();
		}

		private static void OnFormChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			((DynamicControl)o).ResetContent();
		}

		private void ResetContent()
		{
			RootGrid.Children.Clear();
			RootGrid.RowDefinitions.Clear();

			if (Model == null)
			{
				return;
			}

			RootGrid.MinWidth = Model.MinWidth;
			RootGrid.MinHeight = Model.MinHeight;

			int row = 0;

			foreach (IElement element in Model.Elements)
			{
				RootGrid.RowDefinitions.Add(new RowDefinition
				{
					Height = GridLength.Auto
				});

				Presenter presenter = element.ToPresenter();

				Grid.SetRow(presenter, row);

				RootGrid.Children.Add(presenter);

				row++;
			}
		}
	}
}
