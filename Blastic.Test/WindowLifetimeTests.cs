using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blastic.LifetimeManagement;
using Blastic.Services.Windowing;
using NUnit.Framework;

namespace Blastic.Test;

[TestFixture(false)]
[TestFixture(true)]
public class WindowLifetimeTests(bool asynchronous)
{
	[Test]
	public async Task Native_events_drive_lifetime_without_duplicate_commands()
	{
		Probe probe = new(asynchronous);
		int closeRequests = 0;
		using WindowLifetime window = new(probe.Model, () => closeRequests++);

		await window.OnCreated();
		await window.OnCreated();
		await window.OnActivated();
		await window.OnActivated();
		await window.OnDeactivated();
		await window.OnActivated();
		await window.OnClosed();
		await window.OnClosed();

		Assert.That(probe.Events, Is.EqualTo(new[]
		{
			"initialize",
			"activate",
			"deactivate",
			"activate",
			"deactivate",
			"close"
		}));
		Assert.That(probe.IsInitialized, Is.False);
		Assert.That(probe.IsActive, Is.False);
		Assert.That(closeRequests, Is.Zero);
	}

	[Test]
	public async Task Viewmodel_closure_closes_native_window_without_recursion()
	{
		Probe probe = new(asynchronous);
		int closeRequests = 0;
		Task? nativeClosure = null;
		WindowLifetime window = null!;
		using (window = new WindowLifetime(probe.Model, () =>
		{
			closeRequests++;
			nativeClosure = window.OnClosed();
		}))
		{
			await window.OnActivated();
			await probe.Close();
			await nativeClosure!;
		}

		Assert.That(closeRequests, Is.EqualTo(1));
		Assert.That(probe.Events.FindAll(value => value == "close"), Has.Count.EqualTo(1));
		Assert.That(probe.IsInitialized, Is.False);
	}

	[Test]
	public async Task Closed_window_detaches_before_viewmodel_is_reopened()
	{
		Probe probe = new(asynchronous);
		int oldCloseRequests = 0;
		int newCloseRequests = 0;

		using WindowLifetime oldWindow = new(probe.Model, () => oldCloseRequests++);
		await oldWindow.OnActivated();
		await oldWindow.OnClosed();

		using WindowLifetime newWindow = new(probe.Model, () => newCloseRequests++);
		await newWindow.OnActivated();
		await oldWindow.OnDeactivated();
		
		Assert.That(probe.IsActive, Is.True);
		await probe.Close();

		Assert.That(oldCloseRequests, Is.Zero);
		Assert.That(newCloseRequests, Is.EqualTo(1));
	}

	[Test]
	public async Task Disposed_binding_does_not_close_window()
	{
		Probe probe = new(asynchronous);
		int closeRequests = 0;
		WindowLifetime window = new(probe.Model, () => closeRequests++);
		await window.OnActivated();
		window.Dispose();
		await window.OnDeactivated();
		Assert.That(probe.IsActive, Is.True);
		await probe.Close();
		Assert.That(closeRequests, Is.Zero);
	}

	[Test]
	public async Task Native_events_wait_for_pending_async_initialization()
	{
		AsynchronousModel model = new();
		TaskCompletionSource<bool> ready = new();
		model.Lifetime.Initialization.Subscribe(() => ready.Task);
		using WindowLifetime window = new(model, () => Assert.Fail("Native closure must not request another close."));

		Task created = window.OnCreated();
		Task activated = window.OnActivated();
		Task closed = window.OnClosed();
		Assert.That(activated.IsCompleted, Is.False);
		Assert.That(closed.IsCompleted, Is.False);
		ready.SetResult(true);
		await Task.WhenAll(created, activated, closed);

		Assert.That(model.Lifetime.IsInitialized.Value, Is.False);
		Assert.That(model.Lifetime.IsActive.Value, Is.False);
	}

	private sealed class SynchronousModel : IHasLifetime
	{
		public ILifetime Lifetime { get; } = new Lifetime();
	}

	private sealed class AsynchronousModel : IHasAsyncLifetime
	{
		public IAsyncLifetime Lifetime { get; } = new AsyncLifetime();
	}

	private sealed class Probe
	{
		public object Model { get; }
		public List<string> Events { get; } = new();

		public bool IsInitialized => Model is IHasLifetime synchronous
			? synchronous.Lifetime.IsInitialized.Value
			: ((IHasAsyncLifetime)Model).Lifetime.IsInitialized.Value;

		public bool IsActive => Model is IHasLifetime synchronous
			? synchronous.Lifetime.IsActive.Value
			: ((IHasAsyncLifetime)Model).Lifetime.IsActive.Value;

		public Probe(bool asynchronous)
		{
			if (asynchronous)
			{
				AsynchronousModel model = new();
				Model = model;
				model.Lifetime.Initialization.Subscribe(() => Record("initialize"));
				model.Lifetime.Activation.Subscribe(() => Record("activate"));
				model.Lifetime.Deactivation.Subscribe(() => Record("deactivate"));
				model.Lifetime.Closure.Subscribe(() => Record("close"));
			}
			else
			{
				SynchronousModel model = new();
				Model = model;
				model.Lifetime.Initialization.Subscribe(() => Events.Add("initialize"));
				model.Lifetime.Activation.Subscribe(() => Events.Add("activate"));
				model.Lifetime.Deactivation.Subscribe(() => Events.Add("deactivate"));
				model.Lifetime.Closure.Subscribe(() => Events.Add("close"));
			}
		}

		private Task Record(string name)
		{
			Events.Add(name);

			return Task.CompletedTask;
		}

		public Task Close()
		{
			if (Model is IHasLifetime synchronous)
			{
				synchronous.Lifetime.Close();

				return Task.CompletedTask;
			}

			return ((IHasAsyncLifetime)Model).Lifetime.Close();
		}
	}
}