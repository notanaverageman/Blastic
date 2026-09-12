using Blastic.Reactive;

namespace Blastic.DynamicControls.Elements;

public interface ISelectionValueWithLabel
{
	IReadOnlyReactiveProperty<string?> Label { get; }
}

public class SelectionValueWithLabel<T> : ISelectionValueWithLabel
{
	public IReadOnlyReactiveProperty<string?> Label { get; }
	public T Value { get; }

	public SelectionValueWithLabel(IReadOnlyReactiveProperty<string?> label, T value)
	{
		Label = label;
		Value = value;
	}
}