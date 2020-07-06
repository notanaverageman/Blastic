using System;
using System.Windows;
using System.Windows.Controls;
using Blastic.DynamicControls;
using Blastic.Wpf.DynamicControls.Presenters;

namespace Blastic.Wpf.DynamicControls
{
	[TemplatePart(Name = "PART_RootGrid", Type = typeof(Grid))]
	public class DynamicControl : Control
	{
		static DynamicControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DynamicControl), new FrameworkPropertyMetadata(typeof(DynamicControl)));
		}

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

		private Grid _rootGrid;

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_rootGrid = GetTemplateChild("PART_RootGrid") as Grid;

			if (_rootGrid == null)
			{
				throw new ArgumentException("Root grid is not defined in template.");
			}

			ResetContent();
		}

		private static void OnFormChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			((DynamicControl)o).ResetContent();
		}

		private void ResetContent()
		{
			if (_rootGrid == null)
			{
				return;
			}

			_rootGrid.Children.Clear();
			_rootGrid.RowDefinitions.Clear();

			if (Model == null)
			{
				return;
			}

			_rootGrid.MinWidth = Model.MinWidth;
			_rootGrid.MinHeight = Model.MinHeight;

			int row = 0;

			foreach (IElement element in Model.Elements)
			{
				_rootGrid.RowDefinitions.Add(new RowDefinition
				{
					Height = GridLength.Auto
				});

				Presenter presenter = (Presenter)PresenterSource.Instance.CreatePresenter(element);

				Grid.SetRow(presenter, row);

				_rootGrid.Children.Add(presenter);

				row++;
			}
		}
	}
}