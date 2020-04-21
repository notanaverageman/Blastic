using Blastic.Reactive;

namespace Blastic.DynamicControls
{
	public interface IField : IElement
	{
		IReadOnlyReactiveProperty Property { get; }
	}

	public abstract class Field : Element, IField
	{
		public IReadOnlyReactiveProperty Property { get; }

		public Field(IReadOnlyReactiveProperty property)
		{
			Property = property;
		}
	}
}