using System;
using System.Collections;
using System.Reflection;
using AiForms.Renderers;

namespace Blastic.Forms.Sample.Controls
{
	public class PickerCellSelectionColorFix : PickerCell
	{
		private static readonly PropertyInfo DisplayValueProperty;
		private static readonly PropertyInfo SubDisplayValueProperty;
		private static readonly PropertyInfo MergedSelectedListProperty;
		private static readonly MethodInfo InvokeCommandMethod;
		private static readonly MethodInfo GetSelectedItemsTextMethod;
		
		static PickerCellSelectionColorFix()
		{
			DisplayValueProperty = typeof(PickerCell).GetProperty(
				"DisplayValue",
				BindingFlags.Instance | BindingFlags.NonPublic);

			SubDisplayValueProperty = typeof(PickerCell).GetProperty(
				"SubDisplayValue",
				BindingFlags.Instance | BindingFlags.NonPublic);

			MergedSelectedListProperty = typeof(PickerCell).GetProperty(
				"MergedSelectedList",
				BindingFlags.Instance | BindingFlags.NonPublic);

			InvokeCommandMethod = typeof(PickerCell).GetMethod(
				"InvokeCommand",
				BindingFlags.Instance | BindingFlags.NonPublic);

			GetSelectedItemsTextMethod = typeof(PickerCell).GetMethod(
				"GetSelectedItemsText",
				BindingFlags.Instance | BindingFlags.NonPublic);
		}

		public Func<object, object> DisplayValuePublic => (Func<object, object>) DisplayValueProperty.GetValue(this);
		public Func<object, object> SubDisplayValuePublic => (Func<object, object>) SubDisplayValueProperty.GetValue(this);
		public IList MergedSelectedListPublic => (IList)MergedSelectedListProperty.GetValue(this);

		public void InvokeCommandPublic()
		{
			InvokeCommandMethod.Invoke(this, Array.Empty<object>());
		}
		
		public string GetSelectedItemsTextPublic()
		{
			return (string) GetSelectedItemsTextMethod.Invoke(this, Array.Empty<object>());
		}
	}
}