using System.ComponentModel.DataAnnotations;
using Blastic.DynamicControls;
using Blastic.Reactive;

namespace Blastic.Wpf.Sample.Data
{
	public class Customer
	{
		[Key]
		public int Id { get; set; }

		public string Name { get; set; }
		public string Address { get; set; }
	}

	public class CustomerForm
	{
		public IReactiveProperty<int> Id
		{
			get;
		}

		public IReactiveProperty<string> Name
		{
			get;
		}

		public IReactiveProperty<string> Address
		{
			get;
		}

		public CustomerForm(Customer value)
		{
			Id = new ReactiveProperty<int>(value.Id);
			Name = new ReactiveProperty<string>(value.Name);
			Address = new ReactiveProperty<string>(value.Address);
		}

		public DynamicModel ToDynamicModel()
		{
			DynamicModel model = new DynamicModel();
			model.AddNumber(Id);
			model.AddText(Name);
			model.AddText(Address);
			return model;
		}

		public Customer ToCustomer()
		{
			Customer value = new Customer();
			value.Id = Id.Value;
			value.Name = Name.Value;
			value.Address = Address.Value;
			return value;
		}
	}
}