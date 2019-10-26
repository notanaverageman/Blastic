using System;
using System.Windows;
using System.Windows.Controls;
using Blastic.Controls.DynamicControls.Elements;
using Blastic.Execution;

namespace Blastic.Controls.DynamicControls
{
	[TemplatePart(Name = "PART_RootGrid", Type = typeof(Grid))]
	public class DynamicControl : Control
	{
		static DynamicControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DynamicControl), new FrameworkPropertyMetadata(typeof(DynamicControl)));
		}

		public static readonly DependencyProperty ExecutionContextProperty = DependencyProperty.Register(
			nameof(ExecutionContextProperty).Replace("Property", ""),
			typeof(ExecutionContext),
			typeof(DynamicControl),
			new PropertyMetadata(default(ExecutionContext)));
		public ExecutionContext ExecutionContext
		{
			get => (ExecutionContext)GetValue(ExecutionContextProperty);
			set => SetValue(ExecutionContextProperty, value);
		}

		public static readonly DependencyProperty FormProperty = DependencyProperty.Register(
			nameof(FormProperty).Replace("Property", ""),
			typeof(DynamicModel),
			typeof(DynamicControl),
			new PropertyMetadata(default(DynamicModel), OnFormChanged));
		public DynamicModel Form
		{
			get => (DynamicModel)GetValue(FormProperty);
			set => SetValue(FormProperty, value);
		}

		private Grid _rootGrid;

		public void Cancel()
		{
			Form?.Cancel();
		}

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


			if (Form == null)
			{
				return;
			}

			int row = 0;

			foreach (IElement element in Form.Elements)
			{
				_rootGrid.RowDefinitions.Add(new RowDefinition
				{
					Height = GridLength.Auto
				});

				Presenter presenter = element.ToPresenter();

				Grid.SetRow(presenter, row);

				_rootGrid.Children.Add(presenter);

				row++;
			}
		}
	}
}