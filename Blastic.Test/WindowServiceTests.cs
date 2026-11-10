using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Blastic.Avalonia.Services.Windowing;
using Blastic.Avalonia.ViewManagement;
using Blastic.LifetimeManagement;
using Blastic.Reactive;
using Blastic.ViewManagement;
using Blastic.ViewManagement.TypeMappers;
using NUnit.Framework;

namespace Blastic.Test;

[NonParallelizable]
public class WindowServiceTests
{
	private HeadlessUnitTestSession _session = null!;

	[OneTimeSetUp]
	public void StartApplication()
	{
		_session = HeadlessUnitTestSession.StartNew(typeof(TestApplication));
	}

	[OneTimeTearDown]
	public void StopApplication()
	{
		_session.Dispose();
	}

	[Test]
	public async Task Native_close_clears_lifetime_and_cached_view_and_allows_reopening()
	{
		await _session.Dispatch(async () =>
		{
			WindowModel model = new();
			WindowService service = CreateService();
			await service.ShowWindow(model);
			Window first = (Window)model.View.Value!;
			
			Assert.That(model.Lifetime.IsInitialized.Value, Is.True);
			first.Close();
			Assert.That(model.Lifetime.IsInitialized.Value, Is.False);
			Assert.That(model.View.Value, Is.Null);

			await service.ShowWindow(model);
			Window second = (Window)model.View.Value!;
			Assert.That(second, Is.Not.SameAs(first));
			second.Close();

			return true;
		}, CancellationToken.None);
	}

	[Test]
	public async Task Repeated_show_reuses_window_and_lifetime_close_closes_it_once()
	{
		await _session.Dispatch(async () =>
		{
			WindowModel model = new();
			WindowService service = CreateService();
			await service.ShowWindow(model);
			Window window = (Window)model.View.Value!;
			int closed = 0;
			window.Closed += (_, _) => closed++;
			await service.ShowWindow(model);
			Assert.That(model.View.Value, Is.SameAs(window));

			model.Lifetime.Close();
			Assert.That(closed, Is.EqualTo(1));
			Assert.That(window.IsVisible, Is.False);
			Assert.That(model.View.Value, Is.Null);

			return true;
		}, CancellationToken.None);
	}

	private static WindowService CreateService()
	{
		ViewLocator locator = new(new EmptyServices(), new ITypeMapper[]
		{
			new DirectTypeMapper<WindowModel, Window>()
		});

		return new WindowService(locator);
	}

	public sealed class TestApplication
	{
		public static AppBuilder BuildAvaloniaApp()
		{
			return AppBuilder.Configure<Application>().UseHeadless(new AvaloniaHeadlessPlatformOptions());
		}
	}

	private sealed class WindowModel : IHasLifetime, IViewAware
	{
		public ILifetime Lifetime { get; } = new Lifetime();
		public IReactiveProperty<object?> View { get; } = new ReactiveProperty<object?>(null);
	}

	private sealed class EmptyServices : IServiceProvider
	{
		public object? GetService(Type serviceType) => null;
	}
}
