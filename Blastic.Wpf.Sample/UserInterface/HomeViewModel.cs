using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Blastic.Commanding;
using Blastic.DynamicControls;
using Blastic.LifetimeManagement;
using Blastic.Ordering;
using Blastic.Reactive;
using Blastic.Services.Localization;
using Blastic.Services.Notifications;
using Blastic.Wpf.Sample.Data;
using Blastic.Wpf.UserInterface.TabbedMain;
using Microsoft.EntityFrameworkCore;

namespace Blastic.Wpf.Sample.UserInterface
{
	public class HomeViewModel : Screen, IMainTab
	{
		private readonly ILocalizationService _localizationService;
		private readonly INotificationService _notificationService;
		private readonly SampleContext _dbContext;

		public Order Order { get; }
		public bool IsFixed => true;

		public IReadOnlyReactiveProperty<string> Title { get; }

		public AsyncCommand AddCustomerCommand { get; }
		public AsyncCommand RefreshCustomersCommand { get; }

		public ReactiveCollection<Customer> Customers { get; }

		public HomeViewModel(
			ILocalizationService localizationService,
			INotificationService notificationService,
			SampleContext dbContext)
		{
			_localizationService = localizationService;
			_notificationService = notificationService;
			_dbContext = dbContext;
			Order = new Order(1);

			Title = new LocalizableReactiveProperty(localizationService, "Blastic.Sample.Homepage");

			AddCustomerCommand = new AsyncCommand(Test);
			RefreshCustomersCommand = new AsyncCommand(FetchData);

			Customers = new ReactiveCollection<Customer>();

			Lifetime.Initialize.Subscribe(FetchData);
		}

		public async Task FetchData()
		{
			List<Customer> customers = await _dbContext.Customers.ToListAsync();

			Customers.Clear();
			Customers.AddRange(customers);

			_localizationService.Culture = Equals(_localizationService.Culture, CultureInfo.GetCultureInfo("tr-TR"))
				? CultureInfo.GetCultureInfo("en-US")
				: CultureInfo.GetCultureInfo("tr-TR");
		}

		public async Task Test()
		{
			CustomerForm form = new CustomerForm(new Customer());

			form.Name.AddValidator(x => string.IsNullOrEmpty(x) ? "Cannot be empty" : null);
			form.Address.AddValidator(x => string.IsNullOrEmpty(x) ? "Cannot be empty" : null);

			DynamicModel model = form
				.ToDynamicModel()
				.AddOkCancelAction(canExecuteOk: ReactivePropertyExtensions.NoErrors(form.Name, form.Address));

			if (!await ExecutionContext.ShowForm(model))
			{
				return;
			}

			Customer customer = new Customer
			{
				Name = form.Name.Value,
				Address = form.Address.Value
			};

			_dbContext.Customers.Add(customer);
			await _dbContext.SaveChangesAsync();

			Customers.Add(customer);

			await _notificationService.Enqueue(new Notification($"New customer is added with name {form.Name.Value} "));
		}
	}
}