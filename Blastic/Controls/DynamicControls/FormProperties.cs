using MaterialDesignThemes.Wpf;

namespace Blastic.Controls.DynamicControls
{
	public class FormProperties
	{
		public string Label { get; set; }
		public string Help { get; set; }
		public string Mask { get; set; }

		public PackIconKind? Icon { get; set; }

		public bool IsSecret { get; set; }

		public FormProperties WithLabel(string label)
		{
			Label = label;
			return this;
		}

		public FormProperties WithHelp(string help)
		{
			Help = help;
			return this;
		}

		public FormProperties WithIcon(PackIconKind icon)
		{
			Icon = icon;
			return this;
		}

		public FormProperties WithMask(string mask)
		{
			Mask = mask;
			return this;
		}
	}
}