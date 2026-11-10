using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blastic.LifetimeManagement;
using Blastic.Ordering;

namespace Blastic.Services.Windowing;

/// <summary>Forwards native window events to a viewmodel and lifetime closure back to the window.</summary>
public sealed class WindowLifetime : IDisposable
{
	private readonly ILifetime? _lifetime;
	private readonly IAsyncLifetime? _asyncLifetime;
	private readonly Action _closeWindow;
	private readonly SemaphoreSlim _events = new(1, 1);
	private IDisposable? _closeSubscription;
	private bool _closed;
	private bool _closingFromLifetime;

	public WindowLifetime(object viewModel, Action closeWindow)
	{
		_lifetime = (viewModel as IHasLifetime)?.Lifetime;
		_asyncLifetime = _lifetime == null ? (viewModel as IHasAsyncLifetime)?.Lifetime : null;
		_closeWindow = closeWindow;

		if (_lifetime != null)
		{
			_closeSubscription = _lifetime.Closure.Subscribe(CloseWindow, Order.AbsoluteMaximum);
		}
		else if (_asyncLifetime != null)
		{
			_closeSubscription = _asyncLifetime.Closure.Subscribe(() =>
			{
				CloseWindow();
				return Task.CompletedTask;
			}, Order.AbsoluteMaximum);
		}
	}

	public Task OnCreated() => Forward(() =>
	{
		_lifetime?.Initialize();
		return _asyncLifetime?.Initialize() ?? Task.CompletedTask;
	});

	public Task OnActivated() => Forward(() =>
	{
		if (_lifetime != null && !_lifetime.IsActive.Value)
		{
			_lifetime.Activate();
		}

		return _asyncLifetime != null && !_asyncLifetime.IsActive.Value
			? _asyncLifetime.Activate()
			: Task.CompletedTask;
	});

	public Task OnDeactivated() => Forward(() =>
	{
		_lifetime?.Deactivate();
		return _asyncLifetime?.Deactivate() ?? Task.CompletedTask;
	});

	public Task OnClosed()
	{
		bool closingFromLifetime = _closingFromLifetime;

		return Forward(async () =>
		{
			_closed = true;
			Dispose();

			if (!closingFromLifetime)
			{
				_lifetime?.Close();
				await (_asyncLifetime?.Close() ?? Task.CompletedTask);
			}
		});
	}

	private async Task Forward(Func<Task> action)
	{
		await _events.WaitAsync();

		try
		{
			if (!_closed)
			{
				await action();
			}
		}
		finally
		{
			_events.Release();
		}
	}

	private void CloseWindow()
	{
		if (_closed || _closingFromLifetime)
		{
			return;
		}

		_closingFromLifetime = true;

		try
		{
			_closeWindow();
		}
		finally
		{
			_closingFromLifetime = false;
		}
	}

	public void Dispose()
	{
		_closed = true;
		IDisposable? subscription = _closeSubscription;
		_closeSubscription = null;
		if (subscription == null)
		{
			return;
		}

		if (_asyncLifetime?.Closure.IsExecuting.Value == true)
		{
			// AsyncCommand enumerates its subscriptions while executing. Remove ours after it finishes.
			_asyncLifetime.Closure.IsExecuting
				.Where(isExecuting => !isExecuting)
				.Take(1)
				.Subscribe(_ => subscription.Dispose());
		}
		else
		{
			subscription.Dispose();
		}
	}
}