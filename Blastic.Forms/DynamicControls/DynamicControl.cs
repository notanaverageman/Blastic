using System;
using Blastic.DynamicControls;
using Blastic.Forms.DynamicControls.Presenters;
using Xamarin.Forms;

namespace Blastic.Forms.DynamicControls
{
	public class DynamicControl : ContentView
	{
		public static readonly BindableProperty ModelProperty = BindableProperty.Create(
			nameof(ModelProperty).Replace("Property", ""),
			typeof(DynamicModel),
			typeof(DynamicControl),
			default(DynamicModel),
			propertyChanged: OnFormChanged);
		public DynamicModel Model
		{
			get => (DynamicModel)GetValue(ModelProperty);
			set => SetValue(ModelProperty, value);
		}

		private Grid _rootGrid;

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_rootGrid = GetTemplateChild("PART_RootGrid") as Grid;

			if (_rootGrid == null)
			{
				throw new ArgumentException("Root grid is not defined in template.");
			}

			ResetContent();
		}

		private static void OnFormChanged(BindableObject bindable, object oldValue, object newValue)
		{
			((DynamicControl)bindable).ResetContent();
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

			// TODO:
			// _rootGrid.MinWidth = Model.MinWidth;
			// _rootGrid.MinHeight = Model.MinHeight;

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