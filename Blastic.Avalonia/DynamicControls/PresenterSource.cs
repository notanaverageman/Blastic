using Blastic.Avalonia.DynamicControls.Presenters;
using Blastic.DynamicControls;
using Blastic.DynamicControls.Elements;

namespace Blastic.Avalonia.DynamicControls;

public class PresenterSource : IPresenterSource
{
	public static readonly PresenterSource Instance = new();

	private PresenterSource()
	{
	}

	public IPresenter CreatePresenter(IElement element)
	{
		Presenter? presenter = element switch
		{
			ActionElement actionElement    => CreateActionPresenter(actionElement),
			BooleanField                   => CreateBooleanPresenter(),
			GroupElement groupElement      => CreateGroupPresenter(groupElement),
			LabelField                     => CreateLabelPresenter(),
			PasswordField                  => CreatePasswordPresenter(),
			TextField textField            => CreateTextPresenter(textField),
			ISelectionField selectionField => CreateSelectionPresenter(selectionField),
			_ => null
		};

		if (presenter == null)
		{
			throw new ArgumentException($"Unknown element type: {element.GetType()}");
		}

		if (element is IField field)
		{
			presenter.Property = field.Property;
		}

		presenter.Help = element.Help;
		presenter.Label = element.Label;
		presenter.Icon = element.Icon;

		presenter.IsEnabledReactive = element.IsEnabled;

		presenter.Margin = element.Margin.ToMaui();
		presenter.Padding = element.Padding.ToMaui();
		presenter.IconMargin = element.IconMargin;
		presenter.IconSize = element.IconSize;
		presenter.ColumnWidth = element.ColumnWidth;
		presenter.HorizontalAlignment = element.HorizontalAlignment.ToAvalonia();

		presenter.MinWidth = element.MinWidth;
		presenter.MinHeight = element.MinHeight;

		return presenter;
	}

	private Presenter CreateActionPresenter(ActionElement actionElement)
	{
		return new ActionPresenter
		{
			Command = actionElement.Command
		};
	}

	private Presenter CreateBooleanPresenter()
	{
		return new BooleanPresenter();
	}

	private Presenter CreateGroupPresenter(GroupElement groupElement)
	{
		return new GroupPresenter(this, groupElement.Elements);
	}

	private Presenter CreateLabelPresenter()
	{
		return new LabelPresenter();
	}

	private Presenter CreatePasswordPresenter()
	{
		return new PasswordPresenter();
	}

	private Presenter CreateTextPresenter(TextField textField)
	{
		return new TextPresenter
		{
			Mask = textField.Mask
		};
	}

	private Presenter CreateSelectionPresenter(ISelectionField selectionField)
	{
		return new SelectionPresenter(selectionField.SelectedIndex)
		{
			Values = selectionField.Values
		};
	}
}