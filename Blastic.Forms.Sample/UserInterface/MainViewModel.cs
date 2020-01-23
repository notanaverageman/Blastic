using System.Collections.Generic;
using System.Linq;
using Blastic.Forms.UserInterface;
using Blastic.LifetimeManagement;

namespace Blastic.Forms.Sample.UserInterface
{
	public class MainViewModel : ConductorOneActive<IShellTab>
	{
		public MainViewModel(IEnumerable<IShellTab> tabs)
		{
			tabs = tabs
				.OrderBy(x => x.Order)
				.ToList();

			Items.AddRange(tabs);

			ActiveItem.Value = Items.FirstOrDefault();

			Lifetime.Activate.Subscribe(async x =>
			{
				await Activate(ActiveItem.Value, x.Parameter.CancellationToken);
			});
		}
	}
}