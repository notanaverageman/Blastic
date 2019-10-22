using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Blastic.Controls.DynamicControls.Elements.Group
{
	[TemplatePart(Name = "PART_FieldsGrid", Type = typeof(Grid))]
	public class GroupPresenter : Presenter
	{
		static GroupPresenter()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(GroupPresenter), new FrameworkPropertyMetadata(typeof(GroupPresenter)));
		}

		private Grid _fieldsGrid;

		public IEnumerable<IElement> Elements { get; }

		public GroupPresenter(IEnumerable<IElement> elements)
		{
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
				_fieldsGrid.ColumnDefinitions.Add(new ColumnDefinition
				{
					Width = element.ColumnWidth
				});

				Presenter presenter = element.ToPresenter();
				Grid.SetColumn(presenter, column);

				_fieldsGrid.Children.Add(presenter);

				column++;
			}
		}
	}
}