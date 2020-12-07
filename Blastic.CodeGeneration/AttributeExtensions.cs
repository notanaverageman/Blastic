using System;
using System.Reflection;

namespace Blastic.CodeGeneration
{
	public static class AttributeExtensions
	{
		public static TAttribute GetAttribute<TAttribute>(this Assembly assembly) where TAttribute : Attribute
		{
			return (TAttribute)Attribute.GetCustomAttribute(assembly, typeof(TAttribute));
		}
	}
}