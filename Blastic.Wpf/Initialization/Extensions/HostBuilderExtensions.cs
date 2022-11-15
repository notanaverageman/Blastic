using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Blastic.Wpf.Initialization.Extensions
{
	public static class HostBuilderExtensions
	{
		public static IHostBuilder ConfigureBlasticApplication(
			this IHostBuilder hostBuilder,
			Action<BlasticApplicationBuilder>? configureAction = null)
		{
			hostBuilder.ConfigureServices(
				(_, x) =>
				{
					BlasticApplicationBuilder builder = new BlasticApplicationBuilder(x);
					configureAction?.Invoke(builder);

					if (builder.MainViewModelType == null)
					{
						throw new ArgumentNullException(nameof(BlasticApplicationBuilder.MainViewModelType));
					}

					x.AddSingleton(builder);
					x.AddSingleton(builder.MainViewModelType);

					x.AddHostedService<WpfHostedService>();
				});

			return hostBuilder;
		}
	}
}