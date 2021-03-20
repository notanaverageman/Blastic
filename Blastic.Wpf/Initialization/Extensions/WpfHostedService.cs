using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Blastic.Wpf.Initialization.Extensions
{
	public class WpfHostedService : IHostedService
	{
		private readonly WpfApp _app;

		public WpfHostedService(WpfApp app)
		{
			_app = app;

		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			_app.Start();
			return Task.CompletedTask;
		}

		public async Task StopAsync(CancellationToken cancellationToken)
		{
			await _app.Stop();
		}
	}
}