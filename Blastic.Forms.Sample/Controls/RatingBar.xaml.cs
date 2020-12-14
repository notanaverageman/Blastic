using Blastic.Forms.Sample.Icons;
using Xamarin.Forms;

namespace Blastic.Forms.Sample.Controls
{
	public partial class RatingBar
	{
		public static readonly BindableProperty CountProperty = BindableProperty.Create(
			nameof(CountProperty).Replace("Property", ""),
			typeof(int),
			typeof(RatingBar),
			5,
			propertyChanged: CountChanged);
		public int Count
		{
			get => (int)GetValue(CountProperty);
			set => SetValue(CountProperty, value);
		}

		public static readonly BindableProperty RatingProperty = BindableProperty.Create(
			nameof(RatingProperty).Replace("Property", ""),
			typeof(double),
			typeof(RatingBar),
			propertyChanged: RatingChanged);
		public double Rating
		{
			get => (double)GetValue(RatingProperty);
			set => SetValue(RatingProperty, value);
		}

		public static readonly BindableProperty EmptyGlyphProperty = BindableProperty.Create(
			nameof(EmptyGlyphProperty).Replace("Property", ""),
			typeof(string),
			typeof(RatingBar),
			IconFont.StarOutline);
		public string EmptyGlyph
		{
			get => (string)GetValue(EmptyGlyphProperty);
			set => SetValue(EmptyGlyphProperty, value);
		}

		public static readonly BindableProperty HalfGlyphProperty = BindableProperty.Create(
			nameof(HalfGlyphProperty).Replace("Property", ""),
			typeof(string),
			typeof(RatingBar),
			IconFont.StarHalfFull);
		public string HalfGlyph
		{
			get => (string)GetValue(HalfGlyphProperty);
			set => SetValue(HalfGlyphProperty, value);
		}

		public static readonly BindableProperty FullGlyphProperty = BindableProperty.Create(
			nameof(FullGlyphProperty).Replace("Property", ""),
			typeof(string),
			typeof(RatingBar),
			IconFont.Star);
		public string FullGlyph
		{
			get => (string)GetValue(FullGlyphProperty);
			set => SetValue(FullGlyphProperty, value);
		}

		public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(
			nameof(FontFamilyProperty).Replace("Property", ""),
			typeof(string),
			typeof(RatingBar));
		public string FontFamily
		{
			get => (string)GetValue(FontFamilyProperty);
			set => SetValue(FontFamilyProperty, value);
		}

		public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
			nameof(FontSizeProperty).Replace("Property", ""),
			typeof(double),
			typeof(RatingBar),
			new FontSizeConverter().ConvertFromInvariantString("Medium"));
		[TypeConverter(typeof(FontSizeConverter))]
		public double FontSize
		{
			get => (double)GetValue(FontSizeProperty);
			set => SetValue(FontSizeProperty, value);
		}

		public static readonly BindableProperty ColorProperty = BindableProperty.Create(
			nameof(ColorProperty).Replace("Property", ""),
			typeof(Color),
			typeof(RatingBar),
			Color.Yellow);
		public Color Color
		{
			get => (Color)GetValue(ColorProperty);
			set => SetValue(ColorProperty, value);
		}

		public RatingBar()
		{
			InitializeComponent();
			ResetContent();
		}

		private void ResetContent()
		{
			Children.Clear();

			int count = Count;

			if (count == 0)
			{
				UpdateChildrenLayout();
				return;
			}

			DataTemplate dataTemplate = (DataTemplate) Resources["ImageTemplate"];

			for (int i = 0; i < count; i++)
			{
				Image image = (Image) dataTemplate.CreateContent();
				FontImageSource imageSource = (FontImageSource)image.Source;

				imageSource.SetBinding(FontImageSource.FontFamilyProperty, new Binding(nameof(FontFamily), source: this));
				imageSource.SetBinding(FontImageSource.SizeProperty, new Binding(nameof(FontSize), source: this));
				imageSource.SetBinding(FontImageSource.ColorProperty, new Binding(nameof(Color), source: this));
				imageSource.Glyph = EmptyGlyph;
				
				Children.Add(image);
			}

			UpdateChildrenLayout();
		}

		private static void CountChanged(BindableObject bindable, object oldValue, object newValue)
		{
			RatingBar ratingBar = (RatingBar) bindable;
			ratingBar.ResetContent();
		}

		private static void RatingChanged(BindableObject bindable, object oldValue, object newValue)
		{
			RatingBar ratingBar = (RatingBar) bindable;
			double rating = (double) newValue;

			for (int i = 0; i < ratingBar.Children.Count; i++)
			{
				View view = ratingBar.Children[i];
				Image image = (Image) view;
				FontImageSource imageSource = (FontImageSource) image.Source;

				double difference = rating - i;

				imageSource.Glyph = difference switch
				{
					> 0.5 => ratingBar.FullGlyph,
					> 0 => ratingBar.HalfGlyph,
					_ => ratingBar.EmptyGlyph
				};
			}
		}
	}
}