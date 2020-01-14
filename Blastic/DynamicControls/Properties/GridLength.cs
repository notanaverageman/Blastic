namespace Blastic.DynamicControls.Properties
{
    public enum GridUnitType
    {
        Auto = 0,
        Pixel,
        Star,
    }

    public struct GridLength
    {
	    public static GridLength Auto { get; } = new GridLength(1.0, GridUnitType.Auto);

        public GridUnitType UnitType { get; }
        public double Value { get; }

        public GridLength(double pixels) : this(pixels, GridUnitType.Pixel)
        {
        }

        public GridLength(double value, GridUnitType type)
        {
	        UnitType = type;
	        Value = value;
        }
    }
}