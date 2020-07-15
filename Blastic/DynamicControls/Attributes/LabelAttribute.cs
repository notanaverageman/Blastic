using System;

namespace Blastic.DynamicControls.Attributes
{
	[AttributeUsage(AttributeTargets.Property)]
	public class LabelAttribute : Attribute
	{
		public string Value { get; }

		public LabelAttribute(string value)
		{
			Value = value;
		}
	}
}