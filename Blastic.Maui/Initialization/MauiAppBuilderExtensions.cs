using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Blastic.LifetimeManagement;
using Blastic.Maui.ViewManagement;
using Blastic.Platform;
using Blastic.ViewManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Hosting;

namespace Blastic.Maui.Initialization;

public static class MauiAppBuilderExtensions
{
	public static MauiAppBuilder UseBlastic<TApp, TMainViewModel>(this MauiAppBuilder builder, IServiceProvider services)
		where TApp : Application, new()
		where TMainViewModel : class
	{
		SubscribeToBindingContext();

		builder.UseMauiApp(_ =>
		{
			PlatformSpecifics.Current = services.GetRequiredService<IPlatformSpecifics>();
			ViewLocator.Current = services.GetRequiredService<IViewLocator<VisualElement>>();

			TMainViewModel mainViewModel = services.GetRequiredService<TMainViewModel>();
			TApp application = services.GetRequiredService<TApp>();

			Page? mainPage = ViewLocator.Current.Locate(mainViewModel) as Page;
			application.MainPage = mainPage;

			SubscribeToLifecycleEvents(mainPage, mainViewModel);

			return application;
		});

		return builder;
	}

	private static void SubscribeToLifecycleEvents(Page? mainPage, object mainViewModel)
	{
		if (mainPage == null)
		{
			return;
		}

		mainPage.ParentChanged += (_, _) =>
		{
			if (mainPage.Parent is not Window window)
			{
				return;
			}

			ILifetime? lifetime = (mainViewModel as IHasLifetime)?.Lifetime;
			IAsyncLifetime? asyncLifetime = (mainViewModel as IHasAsyncLifetime)?.Lifetime;

			window.Created += async (_, _) =>
			{
				lifetime?.Initialize();
				await (asyncLifetime?.Initialize() ?? Task.CompletedTask);
			};

			window.Activated += async (_, _) =>
			{
				lifetime?.Activate();
				await (asyncLifetime?.Activate() ?? Task.CompletedTask);
			};

			window.Deactivated += async (_, _) =>
			{
				lifetime?.Deactivate();
				await (asyncLifetime?.Deactivate() ?? Task.CompletedTask);
			};

			window.Stopped += async (_, _) =>
			{
				lifetime?.Close();
				await (asyncLifetime?.Close() ?? Task.CompletedTask);
			};
		};
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