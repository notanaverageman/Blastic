using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Blastic.DynamicControls.Properties;
using Blastic.Platform;
using Blastic.Reactive;
using DynamicData;

namespace Blastic.DynamicControls.Elements
{
	public interface ISelectionField
	{
		IReactiveProperty<int> SelectedIndex { get; }
		IObservable<Unit> LabelsChangedObservable { get; }
		IEnumerable Values { get; }
	}

	public class SelectionField<T> : Field, ISelectionField
	{
		public IObservable<Unit> LabelsChangedObservable { get; protected set; }
		public IReactiveProperty<int> SelectedIndex { get; }
		public IReadOnlyList<SelectionValueWithLabel<T>> Values { get; }

		IEnumerable ISelectionField.Values => Values;

		public SelectionField(
			IReactiveProperty<T> property,
			IReadOnlyList<SelectionValueWithLabel<T>> values)
			:
			base(property)
		{
			Values = values;
			IconMargin = new Thickness(0, 16, 8, 0);

			LabelsChangedObservable = values
				.Select(x => x.Label)
				.Merge()
				.Throttle(TimeSpan.FromMilliseconds(50))
				.Select(_ => Unit.Default)
				.ObserveOnUI();

			SelectedIndex = new ReactiveProperty<int>(0);

			Property.Subscribe(x =>
			{
				int index = Values.IndexOf(x, SelectionValueWithLabelComparer.Instance);

				if (index < 0)
				{
					return;
				}

				SelectedIndex.Value = index;
			}, true);

			SelectedIndex.Subscribe(x =>
			{
				if (x < 0 || x >= Values.Count)
				{
					return;
				}

				SelectionValueWithLabel<T> selectionValueWithLabel = Values[x];
				((IReactiveProperty)Property).Value = selectionValueWithLabel.Value;
			});
		}

		private class SelectionValueWithLabelComparer : IEqualityComparer<object?>
		{
			public static readonly SelectionValueWithLabelComparer Instance = new();

			bool IEqualityComparer<object?>.Equals(object? x, object? y)
			{
				if (x is not T value || y is not SelectionValueWithLabel<T> selectionValueWithLabel)
				{
					return false;
				}

				return selectionValueWithLabel.Value?.Equals(value) == true;
			}

			public int GetHashCode(object? obj)
			{
				return obj?.GetHashCode() ?? 0;
			}
		}
	}
}