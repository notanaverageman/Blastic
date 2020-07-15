using System;

namespace Blastic.DynamicControls.Attributes
{
	[AttributeUsage(AttributeTargets.Property)]
	public class LocalizedLabelAttribute : Attribute
	{
		public string Key { get; }

		public LocalizedLabelAttribute(string value)
		{
			Key = value;
		}
	}
}