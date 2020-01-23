using Blastic.Forms.Initialization;
using Blastic.Forms.Initialization.Extensions;
using Blastic.Forms.Sample.UserInterface;

namespace Blastic.Forms.Sample.Initialization.Extensions
{
	public static class ApplicationExtensions
	{
		public static BlasticApplication Initialize(this BlasticApplication application)
		{
			application
				.AddLocalizationSource(Properties.Resources.ResourceManager)
				.AddShellTab<HomeViewModel>()
				.AddShellTab<TestViewModel>();

			return application;
		}
	}
}