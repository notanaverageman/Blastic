using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blastic.LifetimeManagement;
using Blastic.LifetimeManagement.Contexts;
using Blastic.Ordering;
using Blastic.Reactive;
using Blastic.Services.Settings;
using Blastic.Settings;
using DynamicData;

namespace Blastic.Forms.Sample.UserInterface.Settings
{
	public abstract class SettingsSectionViewModel : ConductorAllActive<Setting>
	{
		public abstract IReadOnlyReactiveProperty<string> Title { get; }
		
		public ISettingsStorage SettingsStorage { get; }
		
		protected SettingsSectionViewModel(ISettingsStorage settingsStorage)
		{
			SettingsStorage = settingsStorage;
			Lifetime.Initialization.Subscribe(OnInitialize, Order.AbsoluteMinimum);
		}

		private async Task OnInitialize(
			InitializationContext context,
			CancellationToken cancellationToken)
		{
			List<Setting> settings = GetType()
				.GetProperties()
				.Where(x => typeof(Setting).IsAssignableFrom(x.PropertyType))
				.Select(x => (Setting)x.GetValue(this))
				.ToList();

			ItemsSource.Clear();
			ItemsSource.AddRange(settings);
			
			foreach (Setting setting in settings)
			{
				await setting.Lifetime.Initialize(cancellationToken, context);
			}
		}
	}
}