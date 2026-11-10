using System;
using System.Runtime.CompilerServices;
using Blastic.Services.Windowing;
using Blastic.ViewManagement;
using Microsoft.Maui.Controls;

namespace Blastic.Maui.Services.Windowing;

internal static class WindowLifetimeBinding
{
	private static readonly ConditionalWeakTable<Window, WindowLifetime> Bindings = new();

	public static WindowLifetime Attach(Window window, object viewModel, Action? onClosed = null)
	{
		if (Bindings.TryGetValue(window, out WindowLifetime? existing))
		{
			return existing;
		}

		WindowLifetime lifetime = new(viewModel, () => Application.Current!.CloseWindow(window));
		Bindings.Add(window, lifetime);

		window.Created += async (_, _) => await lifetime.OnCreated();
		window.Activated += async (_, _) => await lifetime.OnActivated();
		window.Deactivated += async (_, _) => await lifetime.OnDeactivated();
		window.Destroying += async (_, _) =>
		{
			if (viewModel is IViewAware viewAware)
			{
				viewAware.View.Value = null;
			}

			await lifetime.OnClosed();
			onClosed?.Invoke();
		};

		return lifetime;
	}
}
