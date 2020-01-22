using System.Collections;
using Xamarin.Forms;

namespace Blastic.Forms.DynamicControls.Presenters
{
	public class SelectionPresenter : Presenter
	{
		public static readonly BindableProperty ValuesProperty = BindableProperty.Create(
			nameof(Values).Replace("Property", ""),
			typeof(IEnumerable),
			typeof(SelectionPresenter),
			default(IEnumerable));
		public IEnumerable Values
		{
			get => (IEnumerable)GetValue(ValuesProperty);
			set => SetValue(ValuesProperty, value);
		}
	}
}