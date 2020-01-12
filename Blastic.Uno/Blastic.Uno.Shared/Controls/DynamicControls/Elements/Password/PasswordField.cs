using Blastic.Reactive;
using Blastic.Uno.Shared.Controls.DynamicControls.Elements.Password;

namespace Blastic.Controls.DynamicControls.Elements.Password
{
	public class PasswordField : Field
	{
		public PasswordField(IReactiveProperty property) : base(property)
		{
		}

		protected override Presenter CreatePresenter()
		{
			return new PasswordPresenter();
		}
	}
}