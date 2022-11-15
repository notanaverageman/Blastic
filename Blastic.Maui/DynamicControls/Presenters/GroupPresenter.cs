using System;
using System.Collections.Generic;
using Blastic.DynamicControls;
using Blastic.DynamicControls.Properties;
using Microsoft.Maui.Controls;
using GridLength = Microsoft.Maui.GridLength;
using IElement = Blastic.DynamicControls.IElement;

namespace Blastic.Maui.DynamicControls.Presenters;

public class GroupPresenter : Presenter
{
	private readonly IPresenterSource _presenterSource;

	private Grid? _fieldsGrid;

	public IEnumerable<IElement> Elements { get; }

	public GroupPresenter(IPresenterSource presenterSource, IEnumerable<IElement> elements)
	{
		_presenterSource = presenterSource;
		Elements = elements;

		// OnApplyTemplate is called in base constructor and since Elements
		// is null no content is added to the children. We trigger ResetContent
		// after setting Elements.
		ResetContent();
	}

	protected override void OnApplyTemplate()
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
				Width = element.ColumnWidth.ToMaui()
			};

			if (element.HorizontalAlignment == HorizontalAlignment.Stretch)
			{
				columnDefinition.Width = GridLength.Star;
			}

			_fieldsGrid.ColumnDefinitions.Add(columnDefinition);

			Presenter presenter = (Presenter)_presenterSource.CreatePresenter(element);
			Grid.SetColumn(presenter, column);

			_fieldsGrid.Children.Add(presenter);

			column++;
		}
	}
}