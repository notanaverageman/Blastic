using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Blastic.DynamicControls;
using IElement = Blastic.DynamicControls.IElement;

namespace Blastic.Avalonia.DynamicControls.Presenters;

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

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);

		_fieldsGrid = e.NameScope.Find<Grid>("PART_FieldsGrid") ?? throw new ArgumentException("Root grid is not defined in template.");

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
				Width = element.ColumnWidth.ToAvalonia()
			};

			if (element.HorizontalAlignment == Blastic.DynamicControls.Properties.HorizontalAlignment.Stretch)
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