using System;
using System.Runtime.CompilerServices;
using Blastic.Services.Windowing;
using Blastic.ViewManagement;
using Avalonia.Controls;

namespace Blastic.Avalonia.Services.Windowing;

internal static class WindowLifetimeBinding
{
	private static readonly ConditionalWeakTable<Window, WindowLifetime> Bindings = new();

	public static WindowLifetime Attach(Window window, object viewModel)
	{
		if (Bindings.TryGetValue(window, out WindowLifetime? existing))
		{
			return existing;
		}

		WindowLifetime lifetime = new(viewModel, () => window.Close());
		Bindings.Add(window, lifetime);

		window.Opened += async (_, _) => await lifetime.OnCreated();
		window.Activated += async (_, _) => await lifetime.OnActivated();
		window.Deactivated += async (_, _) => await lifetime.OnDeactivated();
		window.Closed += async (_, _) =>
		{
			if (viewModel is IViewAware viewAware)
			{
				viewAware.View.Value = null;
			}

			await lifetime.OnClosed();
		};

		return lifetime;
	}
}