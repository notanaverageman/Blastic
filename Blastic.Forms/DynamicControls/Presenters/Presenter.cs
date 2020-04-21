using Blastic.DynamicControls;
using Blastic.Reactive;
using Xamarin.Forms;
using GridLength = Blastic.DynamicControls.Properties.GridLength;
using Thickness = Blastic.DynamicControls.Properties.Thickness;

namespace Blastic.Forms.DynamicControls.Presenters
{
	public class Presenter : ContentView, IPresenter
	{
		public static readonly BindableProperty LabelProperty = BindableProperty.Create(
			nameof(Label),
			typeof(IReadOnlyReactiveProperty<string>),
			typeof(Presenter),
			default(IReadOnlyReactiveProperty<string>));
		public IReadOnlyReactiveProperty<string> Label
		{
			get => (IReadOnlyReactiveProperty<string>)GetValue(LabelProperty);
			set => SetValue(LabelProperty, value);
		}

		public static readonly BindableProperty PropertyProperty = BindableProperty.Create(
			nameof(Property),
			typeof(IReadOnlyReactiveProperty),
			typeof(Presenter),
			default(IReactiveProperty));
		public IReadOnlyReactiveProperty Property
		{
			get => (IReadOnlyReactiveProperty)GetValue(PropertyProperty);
			set => SetValue(PropertyProperty, value);
		}

		public static readonly BindableProperty HelpProperty = BindableProperty.Create(
			nameof(Help),
			typeof(IReadOnlyReactiveProperty<string>),
			typeof(Presenter),
			default(IReadOnlyReactiveProperty<string>));
		public IReadOnlyReactiveProperty<string> Help
		{
			get => (IReadOnlyReactiveProperty<string>)GetValue(HelpProperty);
			set => SetValue(HelpProperty, value);
		}

		public static readonly BindableProperty IconProperty = BindableProperty.Create(
			nameof(Icon),
			typeof(IReadOnlyReactiveProperty<object>),
			typeof
				(Presenter),
			default(IReadOnlyReactiveProperty<object>));
		public IReadOnlyReactiveProperty<object> Icon
		{
			get => (IReadOnlyReactiveProperty<object>)GetValue(IconProperty);
			set => SetValue(IconProperty, value);
		}

		public static readonly BindableProperty IsEnabledReactiveProperty = BindableProperty.Create(
			nameof(IsEnabledReactive),
			typeof(IReactiveProperty<bool>),
			typeof(Presenter),
			default(IReactiveProperty<bool>));
		public IReactiveProperty<bool> IsEnabledReactive
		{
			get => (IReactiveProperty<bool>)GetValue(IsEnabledReactiveProperty);
			set => SetValue(IsEnabledReactiveProperty, value);
		}

		// TODO: BindableProperty?
		public GridLength ColumnWidth { get; set; }
		public Thickness IconMargin { get; set; }
		public double IconSize { get; set; }

		// TODO: Not effective.
		public double MinWidth { get; set; }
		public double MinHeight { get; set; }
	}
}