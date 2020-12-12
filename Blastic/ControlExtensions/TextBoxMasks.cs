namespace Blastic.ControlExtensions
{
	public class TextBoxMasks
	{
		/// <summary>
		/// A regex expression to match integers.
		/// </summary>
		public const string IntegerMask = @"(^[+-]?[1-9]\d*$|^0$|^$)";

		/// <summary>
		/// A regex expression to match floating point numbers.
		/// </summary>
		public const string FloatingPointMask = @"^([+-]?(?:[[:d:]]+\.?|[[:d:]]*\.[[:d:]]+))(?:[Ee][+-]?[[:d:]]+)?$";
	}
}