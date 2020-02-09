using System;
using System.Collections;
using System.Reactive.Linq;
using Blastic.DynamicControls;
using Blastic.DynamicControls.Elements;
using Blastic.Forms.DynamicControls.Presenters;
using Blastic.Reactive;

namespace Blastic.Forms.DynamicControls
{
	public class PresenterSource : IPresenterSource
	{
		public static readonly PresenterSource Instance = new PresenterSource();

		private PresenterSource()
		{
		}

		public IPresenter CreatePresenter(IElement element)
		{
			Presenter presenter = element switch
			{
				ActionElement actionElement => CreateActionPresenter(actionElement),
				BooleanField _ => CreateBooleanPresenter(),
				GroupElement groupElement => CreateGroupPresenter(groupElement),
				LabelField _ => CreateLabelPresenter(),
				PasswordField _ => CreatePasswordPresenter(),
				TextField textField => CreateTextPresenter(textField),
				_ => null
			};

			if (presenter == null &&
			    element.GetType().IsGenericType &&
			    element.GetType().GetGenericTypeDefinition() == typeof(SelectionField<>))
			{
				presenter = new SelectionPresenter
				{
					Values = (IEnumerable) element.GetType()
						.GetProperty(nameof(SelectionField<object>.Values))
						.GetValue(element)
				};
			}

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

			presenter.Margin = element.Margin.ToXamarin();
			presenter.Padding = element.Padding.ToXamarin();
			presenter.IconMargin = element.IconMargin;
			presenter.IconSize = element.IconSize;
			presenter.ColumnWidth = element.ColumnWidth;
			presenter.HorizontalOptions = element.HorizontalAlignment.ToXamarin();

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
				Mask = textField.Mask,
                Keyboard = textField.Keyboard.Select(ToXamarinKeyboard).ToReadOnlyReactiveProperty()
			};
		}

        private Xamarin.Forms.Keyboard ToXamarinKeyboard(Keyboard keyboard)
        {
            return keyboard switch
            {
                Keyboard.Text => Xamarin.Forms.Keyboard.Text,
                Keyboard.Chat => Xamarin.Forms.Keyboard.Chat,
                Keyboard.Default => Xamarin.Forms.Keyboard.Default,
                Keyboard.Email => Xamarin.Forms.Keyboard.Email,
                Keyboard.Numeric => Xamarin.Forms.Keyboard.Numeric,
                Keyboard.Plain => Xamarin.Forms.Keyboard.Plain,
                Keyboard.Telephone => Xamarin.Forms.Keyboard.Telephone,
                Keyboard.Url => Xamarin.Forms.Keyboard.Url,
                _ => throw new ArgumentOutOfRangeException(nameof(keyboard), keyboard, null)
            };
        }
	}
}