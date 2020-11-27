using Xamarin.Forms;

namespace Blastic.Forms.Sample.UserInterface
{
	public partial class MainView
	{
		public MainView()
		{
			InitializeComponent();
		}

		protected override Page CreateDefault(object item)
		{
			return new ContentPage();
		}
	}
}