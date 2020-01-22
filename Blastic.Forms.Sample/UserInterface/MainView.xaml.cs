using Xamarin.Forms.Xaml;

namespace Blastic.Forms.Sample.UserInterface
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainView
	{
		public MainView()
		{
			InitializeComponent();

			BindingContext = new MainViewModel();
		}
	}
}