using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Blastic.DynamicControls;

namespace Blastic.Wpf.DynamicControls.Presenters
{
	[TemplatePart(Name = "PART_FieldsGrid", Type = typeof(Grid))]
	public class GroupPresenter : Presenter
	{
		private readonly IPresenterSource _presenterSource;

		static GroupPresenter()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(GroupPresenter), new FrameworkPropertyMetadata(typeof(GroupPresenter)));
		}

		private Grid? _fieldsGrid;

		public IEnumerable<IElement> Elements { get; }

		public GroupPresenter(
			IPresenterSource presenterSource,
			IEnumerable<IElement> elements)
		{
			_presenterSource = presenterSource;
			Elements = elements;
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_fieldsGrid = GetTemplateChild("PART_FieldsGrid") as Grid;

			if (_fieldsGrid == null)
			{
				throw new ArgumentException("Root grid is not defined in template.");
			}

			ResetContent();
		}

		private void ResetContent()
		{
			if (_fieldsGrid == null)
			{
				return;
			}

			_fieldsGrid.Children.Clear();
			_fieldsGrid.ColumnDefinitions.Clear();

			int column = 0;

			foreach (IElement element in Elements)
			{
				ColumnDefinition columnDefinition = new()
				{
					Width = element.ColumnWidth.ToWpf()
				};

				if (element.HorizontalAlignment == Blastic.DynamicControls.Properties.HorizontalAlignment.Stretch)
				{
					columnDefinition.Width = new GridLength(1, GridUnitType.Star);
				}

				_fieldsGrid.ColumnDefinitions.Add(columnDefinition);

				Presenter presenter = (Presenter) _presenterSource.CreatePresenter(element);
				Grid.SetColumn(presenter, column);

				_fieldsGrid.Children.Add(presenter);

				column++;
			}
		}
	}
}