using System.Linq;
using Blastic.Commanding;
using Blastic.DynamicControls;
using Blastic.DynamicControls.Properties;
using Blastic.Reactive;

namespace Blastic.Forms.Sample.UserInterface
{
	public class MainViewModel
	{
		public HomepageViewModel Homepage { get; }

		public Command TestCommand { get; }
		public IReactiveProperty<DynamicModel> Form { get; }

		public MainViewModel()
		{
			Homepage = new HomepageViewModel();
			Form = new ReactiveProperty<DynamicModel>();

			TestCommand = new Command()
				.WithSubscribe(c =>
				{
					ReactiveProperty<string> name = new ReactiveProperty<string>();
					ReactiveProperty<string> password = new ReactiveProperty<string>();
					ReactiveProperty<int> age = new ReactiveProperty<int>();
					ReactiveProperty<bool> boolean = new ReactiveProperty<bool>();
					Command command = new Command(boolean);

					int asd = 0;
					command.Subscribe(() =>
					{
						asd++;
						name.Value = asd.ToString();
					});

					Form.Value = new DynamicModel()
						.AddLabel(name)
						.AddSelection(age, new ReactiveCollection<int>(Enumerable.Range(1, 20)), x => x
							.WithLabel("Ages"))
						.AddGroup(x => x
							.WithHelp("Some help content.")
							.AddText(name, y => y
								.WithLabel("File path")
								.WithColumnWidth(new GridLength(1, GridUnitType.Star)))
							.AddAction(command, y => y
								.WithLabel("Test")))
						.AddText(name, x => x
							.WithLabel("Name")
							.WithHelp("Name of the user."))
						.AddPassword(password, x => x
							.WithLabel("Password")
							.WithHelp("Password of the user."))
						.AddNumber(age, x => x
							.WithLabel("Age")
							.WithHelp("Age of the user."))
						.AddBoolean(boolean, x => x
							.WithLabel("Some check")
							.WithHelp("Some check for the user."))
						.AddAction(command, x => x
							.WithLabel("Some Button")
							.WithIconMargin(new Thickness(0, 0, 8, 0))
							.WithHorizontalAlignment(HorizontalAlignment.Right));
				});
		}
	}
}