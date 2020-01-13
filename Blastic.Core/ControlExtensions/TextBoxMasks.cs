namespace Blastic.ControlExtensions
{
	public class TextBoxMasks
	{
		public const string IntegerMask = @"(^[+-]?[1-9]\d*$|^0$|^$)";
		public const string FloatingPointMask = @"^([+-]?(?:[[:d:]]+\.?|[[:d:]]*\.[[:d:]]+))(?:[Ee][+-]?[[:d:]]+)?$";
	}
}