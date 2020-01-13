namespace Blastic.DynamicControls
{
	public interface IPresenterSource
	{
		IPresenter CreatePresenter(IElement element);
	}
}