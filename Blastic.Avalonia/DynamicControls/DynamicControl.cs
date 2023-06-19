using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Blastic.Avalonia.DynamicControls.Presenters;
using Blastic.DynamicControls;
using IElement = Blastic.DynamicControls.IElement;

namespace Blastic.Avalonia.DynamicControls;

public class DynamicControl : ContentControl
{
	public static readonly AvaloniaProperty ModelProperty = AvaloniaProperty.Register<DynamicControl, DynamicModel>(nameof(Model));

	public DynamicModel? Model
	{
		get => (DynamicModel?)GetValue(ModelProperty);
		set => SetValue(ModelProperty, value);
	}

	static DynamicControl()
	{
		ModelProperty.Changed.Subscribe(OnFormChanged);
	}

	private Grid? _rootGrid;

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);

		_rootGrid = e.NameScope.Find<Grid>("PART_RootGrid") ?? throw new ArgumentException("Root grid is not defined in template.");
		
		ResetContent();
	}

	private static void OnFormChanged(AvaloniaPropertyChangedEventArgs args)
	{
		((DynamicControl)args.Sender).ResetContent();
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