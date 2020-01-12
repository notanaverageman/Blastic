using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Blastic.Controls.DynamicControls.Elements;

namespace Blastic.Uno.Shared.Controls.DynamicControls.Elements.Group
{
	public sealed partial class GroupPresenter
	{
		private Grid _rootGrid;

		public IEnumerable<IElement> Elements { get; }

		public GroupPresenter()
		{
			InitializeComponent();
		}

		public GroupPresenter(IEnumerable<IElement> elements)
		{
			InitializeComponent();
			Elements = elements;

			ResetContent();
		}

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

		private void ResetContent()
		{
			_rootGrid.Children.Clear();
			_rootGrid.ColumnDefinitions.Clear();

			int column = 0;

			foreach (IElement element in Elements)
			{
				_rootGrid.ColumnDefinitions.Add(new ColumnDefinition
				{
					Width = element.ColumnWidth
				});

				Presenter presenter = element.ToPresenter();
				Grid.SetColumn(presenter, column);

				_rootGrid.Children.Add(presenter);

				column++;
			}
		}
	}
}