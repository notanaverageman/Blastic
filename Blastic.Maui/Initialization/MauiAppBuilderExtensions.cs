using System;
using System.ComponentModel;
using Blastic.Maui.ViewManagement;
using Blastic.Maui.Services.Windowing;
using Blastic.Platform;
using Blastic.ViewManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Hosting;

namespace Blastic.Maui.Initialization;

public static class MauiAppBuilderExtensions
{
	public static MauiAppBuilder UseBlastic<TApp, TMainViewModel>(
		this MauiAppBuilder builder,
		IServiceProvider services,
		bool shutdownOnMainWindowClose = false)
		where TApp : Application, new()
		where TMainViewModel : class
	{
		SubscribeToBindingContext();

		builder.Services.AddSingleton<IWindowCreator>(new MainWindowCreator<TMainViewModel>(services, shutdownOnMainWindowClose));

		builder.UseMauiApp(_ =>
		{
			PlatformSpecifics.Current = services.GetRequiredService<IPlatformSpecifics>();
			ViewLocator.Current = services.GetRequiredService<IViewLocator<VisualElement>>();

			return services.GetRequiredService<TApp>();
		});

		return builder;
	}

	private sealed class MainWindowCreator<TMainViewModel>(IServiceProvider services, bool shutdownOnMainWindowClose) : IWindowCreator where TMainViewModel : class
	{
		public Window CreateWindow(Application app, IActivationState? activationState)
		{
			TMainViewModel mainViewModel = services.GetRequiredService<TMainViewModel>();
			Page mainPage = ViewLocator.Current.Locate(mainViewModel) as Page
				?? throw new InvalidOperationException("The main view must be a Page.");

			Window window = new(mainPage);
			WindowLifetimeBinding.Attach(window, mainViewModel, shutdownOnMainWindowClose ? app.Quit : null);

			return window;
		}
	}

	private static void SubscribeToBindingContext()
	{
		ViewHandler.ViewMapper.Add(nameof(BindableObject.BindingContext), (_, view) =>
		{
			if (view is not BindableObject bindable)
			{
				return;
			}

			object? bindingContext = GetExplicitBindingContext(bindable);

			if (bindingContext is not IViewAware viewAware)
			{
				return;
			}

			viewAware.View.Value = view;

			bindable.PropertyChanged += OnPropertyChanged;

			void OnPropertyChanged(object? sender, PropertyChangedEventArgs args)
			{
				if (args.PropertyName != nameof(BindableObject.BindingContext))
				{
					return;
				}

				if (GetExplicitBindingContext(bindable) != null)
				{
					return;
				}

				viewAware.View.Value = null;
				bindable.PropertyChanged -= OnPropertyChanged;
			}
		});

		static object? GetExplicitBindingContext(BindableObject bindableObject)
		{
			return bindableObject.GetValue(BindableObject.BindingContextProperty);
		}
	}
}
