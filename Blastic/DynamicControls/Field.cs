using Blastic.Reactive;

namespace Blastic.DynamicControls
{
	public interface IField : IElement
	{
		IReactiveProperty Property { get; }
	}

	public abstract class Field : Element, IField
	{
		public IReactiveProperty Property { get; }

		public Field(IReactiveProperty property)
		{
			Property = property;
		}
	}
}