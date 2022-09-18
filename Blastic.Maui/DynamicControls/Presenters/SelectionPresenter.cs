using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive;
using Blastic.Reactive;
using Microsoft.Maui.Controls;

namespace Blastic.Maui.DynamicControls.Presenters;

public class SelectionPresenter : Presenter
{
	public static readonly BindableProperty ValuesProperty = BindableProperty.Create(
		nameof(ValuesProperty).Replace("Property", ""),
		typeof(IEnumerable),
		typeof(SelectionPresenter));
	public IEnumerable Values
	{
		get => (IEnumerable)GetValue(ValuesProperty);
		set => SetValue(ValuesProperty, value);
	}

	public IReactiveProperty<int> SelectedIndex { get; }

	private Picker? _picker;

	public SelectionPresenter(IObservable<Unit> labelsChangedObservable, IReactiveProperty<int> selectedIndex)
	{
		SelectedIndex = selectedIndex;

		labelsChangedObservable.Subscribe(_ =>
		{
			if (_picker == null)
			{
				return;
			}

			int currentSelectedIndex = _picker.SelectedIndex;

			// TODO: Remove when following are solved
			// https://github.com/dotnet/maui/issues/9739
			// https://github.com/dotnet/maui/issues/9239
			List<object> itemsSource = new();

			foreach (object o in _picker.ItemsSource)
			{
				itemsSource.Add(o);
			}

			_picker.ItemsSource = itemsSource.ToArray();
			_picker.ItemsSource = itemsSource;

			_picker.SelectedIndex = currentSelectedIndex;
		});
	}

	protected override void OnApplyTemplate()
	{
		base.OnApplyTemplate();

		if (GetTemplateChild("Picker") is not Picker picker)
		{
			throw new ArgumentException("Can't get Picker from template.");
		}

		_picker = picker;
	}
}