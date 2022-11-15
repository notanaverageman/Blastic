using Blastic.Reactive;

namespace Blastic.DynamicControls.Elements;

public class SelectionValueWithLabel<T>
{
	public IReadOnlyReactiveProperty<string?> Label { get; }
	public T Value { get; }

	public SelectionValueWithLabel(IReadOnlyReactiveProperty<string?> label, T value)
	{
		Label = label;
		Value = value;
	}
}