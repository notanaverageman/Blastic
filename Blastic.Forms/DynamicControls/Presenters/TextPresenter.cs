using Blastic.Reactive;
using Xamarin.Forms;

namespace Blastic.Forms.DynamicControls.Presenters
{
	public class TextPresenter : Presenter
	{
        public static readonly BindableProperty MaskProperty = BindableProperty.Create(
            nameof(Mask),
            typeof(IReadOnlyReactiveProperty<string>),
            typeof(TextPresenter),
            default(IReadOnlyReactiveProperty<string>));
        public IReadOnlyReactiveProperty<string> Mask {
            get => (IReadOnlyReactiveProperty<string>)GetValue(MaskProperty);
            set => SetValue(MaskProperty, value);
        }

        public static readonly BindableProperty KeyboardProperty = BindableProperty.Create(
            nameof(Keyboard),
            typeof(IReadOnlyReactiveProperty<Keyboard>),
            typeof(TextPresenter),
            default(IReadOnlyReactiveProperty<Keyboard>));
        public IReadOnlyReactiveProperty<Keyboard> Keyboard {
            get => (IReadOnlyReactiveProperty<Keyboard>)GetValue(KeyboardProperty);
            set => SetValue(KeyboardProperty, value);
        }
    }
}